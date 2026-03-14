# Copilot Instructions

## Diretrizes de projeto
- O campo UpdatedAt em User/BaseEntity só é setado quando há uma atualização real (via Deactivate, Activate, UpdatePassword, UpdateRole). Não deve ser inicializado na criação - deve permanecer null até a primeira alteração.