#!/usr/bin/env bash
set -euo pipefail

echo "Deleting namespace fcg-users"
kubectl delete namespace fcg-users --ignore-not-found=true
