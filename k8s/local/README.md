# FCG.Users local Kubernetes

Ambiente local autocontido para rodar o `fcg-users` no Kind pelo WSL e conectar pelo Lens no Windows.

Este diretorio nao usa os manifests de producao em `k8s/*.yaml` e nao depende do `fcg-orchestration`.

## Requisitos

- WSL Ubuntu com Docker acessivel.
- `kind`
- `kubectl`
- Portas livres no Windows/WSL:
  - `16443` para a API do Kubernetes.
  - `5050` para a API do `fcg-users`.

## Subir o ambiente

Execute pelo WSL:

```bash
cd /mnt/c/dev/study/fiap/fase04/fcg-users
bash k8s/local/up.sh
```

O script:

- cria o cluster Kind `fcg-users-local`, se ele ainda nao existir;
- builda a imagem `fcg-users:local`;
- carrega a imagem no Kind;
- cria o secret local diretamente no Kubernetes;
- sobe SQL Server, inicializa o database `fcg_user`, sobe Kafka e sobe a API;
- gera o kubeconfig para o Lens em `k8s/local/.generated/kubeconfig-lens.yaml`.

Endpoints:

- Health: `http://localhost:5050/health`
- Swagger: `http://localhost:5050/swagger`

## Lens no Windows

Apos o `up.sh`, importe este arquivo no Lens:

```text
C:\dev\study\fiap\fase04\fcg-users\k8s\local\.generated\kubeconfig-lens.yaml
```

O cluster deve aparecer como `kind-fcg-users-local`, ja apontando para o namespace `fcg-system`.

Se o Lens nao conectar no `127.0.0.1:16443`, confirme se o WSL esta com localhost forwarding habilitado. No Windows, o arquivo `%UserProfile%\.wslconfig` deve conter:

```ini
[wsl2]
localhostForwarding=true
```

Depois reinicie o WSL:

```powershell
wsl --shutdown
```

Abra o WSL novamente e rode `bash k8s/local/up.sh`.

## Comandos uteis

```bash
kubectl -n fcg-system get pods
kubectl -n fcg-system logs deployment/fcg-users -f
kubectl -n fcg-system describe pod -l app.kubernetes.io/name=fcg-users
kubectl -n fcg-system rollout restart deployment/fcg-users
curl http://localhost:5050/health
curl -L http://localhost:5050/swagger
```

Para usar um password/JWT local diferente sem commitar secrets:

```bash
FCG_USERS_LOCAL_SQL_PASSWORD='OutraSenha123!' \
FCG_USERS_LOCAL_JWT_SECRET_KEY='outra-chave-local-com-mais-de-32-caracteres' \
bash k8s/local/up.sh
```

## Derrubar

Remover somente os recursos do namespace:

```bash
bash k8s/local/down.sh
```

Remover o cluster Kind inteiro:

```bash
bash k8s/local/down.sh --cluster
```

