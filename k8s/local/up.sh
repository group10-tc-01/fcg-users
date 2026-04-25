#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
MANIFEST_DIR="$SCRIPT_DIR/manifests"
GENERATED_DIR="$SCRIPT_DIR/.generated"

CLUSTER_NAME="${CLUSTER_NAME:-fcg-users-local}"
NAMESPACE="${NAMESPACE:-fcg-system}"
IMAGE_NAME="${IMAGE_NAME:-fcg-users:local}"
SQL_PASSWORD="${FCG_USERS_LOCAL_SQL_PASSWORD:-FcgUsersLocal123!}"
JWT_SECRET_KEY="${FCG_USERS_LOCAL_JWT_SECRET_KEY:-local-development-jwt-secret-key-for-fcg-users-1234567890}"
KUBECONFIG_FILE="$GENERATED_DIR/kubeconfig-lens.yaml"

require_command() {
  local command_name="$1"

  if ! command -v "$command_name" >/dev/null 2>&1; then
    echo "Missing required command: $command_name" >&2
    exit 1
  fi
}

wait_for_job() {
  local job_name="$1"
  local timeout="$2"

  if ! kubectl -n "$NAMESPACE" wait --for=condition=complete "job/$job_name" --timeout="$timeout"; then
    echo ""
    echo "Job $job_name failed or timed out. Recent logs:"
    kubectl -n "$NAMESPACE" logs "job/$job_name" --tail=100 || true
    exit 1
  fi
}

require_command docker
require_command kind
require_command kubectl

if ! docker info >/dev/null 2>&1; then
  echo "Docker is not running or is not available from WSL." >&2
  exit 1
fi

cd "$REPO_ROOT"

if ! kind get clusters 2>/dev/null | grep -Fxq "$CLUSTER_NAME"; then
  echo "Creating Kind cluster: $CLUSTER_NAME"
  kind create cluster --name "$CLUSTER_NAME" --config "$SCRIPT_DIR/kind-cluster-config.yaml"
else
  echo "Kind cluster already exists: $CLUSTER_NAME"
  kubectl config use-context "kind-$CLUSTER_NAME" >/dev/null
fi

echo "Building Docker image: $IMAGE_NAME"
docker build -t "$IMAGE_NAME" -f src/FCG.Users.WebApi/Dockerfile .

echo "Loading image into Kind: $IMAGE_NAME"
kind load docker-image "$IMAGE_NAME" --name "$CLUSTER_NAME"

echo "Applying namespace"
kubectl apply -f "$MANIFEST_DIR/00-namespace.yaml"

echo "Applying local secret in Kubernetes"
kubectl -n "$NAMESPACE" create secret generic fcg-users-local-secrets \
  --from-literal=sqlserver-sa-password="$SQL_PASSWORD" \
  --from-literal=ConnectionStrings__DefaultConnection="Server=sqlserver-service;Database=fcg_user;User Id=sa;Password=$SQL_PASSWORD;TrustServerCertificate=True;" \
  --from-literal=JwtSettings__SecretKey="$JWT_SECRET_KEY" \
  --dry-run=client -o yaml | kubectl apply -f -

echo "Applying config and SQL Server"
kubectl apply \
  -f "$MANIFEST_DIR/01-configmap.yaml" \
  -f "$MANIFEST_DIR/02-sqlserver.yaml"

echo "Waiting for SQL Server"
kubectl -n "$NAMESPACE" rollout status deployment/sqlserver --timeout=300s

echo "Initializing local database"
kubectl -n "$NAMESPACE" delete job sqlserver-init --ignore-not-found=true
kubectl apply -f "$MANIFEST_DIR/03-sqlserver-init-job.yaml"
wait_for_job sqlserver-init 300s

echo "Applying Kafka"
kubectl apply -f "$MANIFEST_DIR/04-kafka.yaml"

echo "Waiting for Kafka"
kubectl -n "$NAMESPACE" rollout status deployment/kafka --timeout=300s

echo "Applying FCG.Users"
kubectl apply -f "$MANIFEST_DIR/05-fcg-users.yaml"

echo "Waiting for FCG.Users"
kubectl -n "$NAMESPACE" rollout status deployment/fcg-users --timeout=300s

echo "Generating kubeconfig for Lens"
mkdir -p "$GENERATED_DIR"
kind get kubeconfig --name "$CLUSTER_NAME" > "$KUBECONFIG_FILE"
kubectl config set-context --kubeconfig "$KUBECONFIG_FILE" "kind-$CLUSTER_NAME" --namespace="$NAMESPACE" >/dev/null

echo ""
echo "FCG.Users local Kubernetes is ready."
echo "API: http://localhost:5050"
echo "Health: http://localhost:5050/health"
echo "Swagger: http://localhost:5050/swagger"
echo ""
echo "Lens kubeconfig:"
if command -v wslpath >/dev/null 2>&1; then
  echo "  $(wslpath -w "$KUBECONFIG_FILE")"
else
  echo "  $KUBECONFIG_FILE"
fi
echo ""
echo "Useful commands:"
echo "  kubectl -n $NAMESPACE get pods"
echo "  kubectl -n $NAMESPACE logs deployment/fcg-users -f"

