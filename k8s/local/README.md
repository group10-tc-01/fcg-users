# FCG.Users local Kubernetes

Esta pasta contem somente os manifests locais da aplicacao `fcg-users`.

A infra compartilhada fica em:

```text
fcg-orchestration/fase-04/k8s
```

Suba a infra primeiro:

```bash
cd fcg-orchestration/fase-04/k8s
bash up.sh
```

Para recriar apenas o `fcg-users`:

```bash
cd fcg-users
bash k8s/local/up.sh
```

Para remover apenas o namespace da aplicacao:

```bash
bash k8s/local/down.sh
```

Comandos uteis:

```bash
kubectl get pods -n fcg-users
kubectl logs -n fcg-users deployment/fcg-users -f
kubectl describe pod -n fcg-users -l app.kubernetes.io/name=fcg-users
curl http://localhost:5050/health
```
