#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CLUSTER_NAME="${CLUSTER_NAME:-fcg-users-local}"
NAMESPACE="${NAMESPACE:-fcg-system}"

if ! command -v kind >/dev/null 2>&1; then
  echo "Missing required command: kind" >&2
  exit 1
fi

if ! command -v kubectl >/dev/null 2>&1; then
  echo "Missing required command: kubectl" >&2
  exit 1
fi

if ! kind get clusters 2>/dev/null | grep -Fxq "$CLUSTER_NAME"; then
  echo "Kind cluster does not exist: $CLUSTER_NAME"
  exit 0
fi

kubectl config use-context "kind-$CLUSTER_NAME" >/dev/null

if [[ "${1:-}" == "--cluster" ]]; then
  echo "Deleting Kind cluster: $CLUSTER_NAME"
  kind delete cluster --name "$CLUSTER_NAME"
  rm -f "$SCRIPT_DIR/.generated/kubeconfig-lens.yaml"
  rmdir "$SCRIPT_DIR/.generated" 2>/dev/null || true
else
  echo "Deleting namespace: $NAMESPACE"
  kubectl delete namespace "$NAMESPACE" --ignore-not-found=true
  echo "Cluster kept. Use 'bash k8s/local/down.sh --cluster' to delete it too."
fi
