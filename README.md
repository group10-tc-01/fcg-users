# 🔐 FCG.Users - API de Identidade e Autenticação

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Responsabilidade](#-responsabilidade)
- [Arquitetura](#-arquitetura)
- [Tecnologias e Bibliotecas](#-tecnologias-e-bibliotecas)
- [Modelo de Dados](#-modelo-de-dados)
- [Regras de Negócio](#-regras-de-negócio)
- [Endpoints da API](#-endpoints-da-api)
- [Eventos](#-eventos)
- [Configuração e Execução](#-configuração-e-execução)

---

## 🎯 Sobre o Projeto

**FCG.Users** é uma API RESTful desenvolvida em .NET 8 para gerenciamento completo de identidade, autenticação e autorização de usuários. A aplicação implementa autenticação baseada em **JWT (JSON Web Tokens)** com suporte a **Refresh Tokens**, seguindo as melhores práticas de segurança e arquitetura de software.

### 🚀 Responsabilidade

A API é responsável por:

- ✅ **Cadastro e gerenciamento de usuários**
- 🔑 **Autenticação com JWT (Access Token e Refresh Token)**
- 👥 **Gestão de perfis de acesso (User e Admin)**
- 🔒 **Controle de autorização baseado em roles**
- 📨 **Publicação de eventos de domínio** (Event-Driven Architecture)
- 🔐 **Criptografia de senhas com BCrypt**
- ⚙️ **Renovação automática de tokens de acesso**

---

## 🏛️ Arquitetura

A aplicação segue os princípios da **Clean Architecture**, garantindo separação de responsabilidades, testabilidade e manutenibilidade do código.

### Estrutura de Camadas

```
┌─────────────────────────────────────────┐
│         FCG.Users.WebApi                │  ← Camada de Apresentação (API REST)
│   Controllers, Middlewares, Filters    │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│       FCG.Users.Application             │  ← Camada de Aplicação (Use Cases)
│   UseCases, Validations, DTOs          │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         FCG.Users.Domain                │  ← Camada de Domínio (Regras de Negócio)
│   Entities, Exceptions, Events         │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│      FCG.Users.Infrastructure.*         │  ← Camada de Infraestrutura
│  SqlServer, Kafka, Auth (JWT/BCrypt)   │
└─────────────────────────────────────────┘
```

### Camadas do Projeto

#### 1️⃣ **Domain** (`FCG.Users.Domain`)
- Entidades de negócio: `User`, `RefreshToken`
- Exceções de domínio: `DomainException`, `NotFoundException`, `ConflictException`, etc.
- Eventos de domínio: `IDomainEvent`
- Abstrações: `BaseEntity`, `IUnitOfWork`

#### 2️⃣ **Application** (`FCG.Users.Application`)
- **Use Cases** (CQRS): Commands e Queries
- **Validações** com FluentValidation
- **Abstrações**: Repositories, Services, Messaging
- Configurações: `JwtSettings`

#### 3️⃣ **Infrastructure**
- **SqlServer** (`FCG.Users.Infrastructure.SqlServer`): Persistência com Entity Framework Core
- **Auth** (`FCG.Users.Infrastructure.Auth`): Implementação JWT e BCrypt
- **Kafka** (`FCG.Users.Infrastructure.Kafka`): Produção e consumo de eventos
- **Messages** (`FCG.Users.Messages`): Recursos de mensagens da aplicação

#### 4️⃣ **Presentation** (`FCG.Users.WebApi`)
- Controllers versionados (`/v1/...`)
- Middlewares customizados (Exception Handler, Correlation ID)
- Health Checks
- Swagger/OpenAPI

---

## 🛠️ Tecnologias e Bibliotecas

### Core Framework
- **.NET 8** - Framework principal
- **C# 12** - Linguagem de programação

### Comunicação Assíncrona
- **Apache Kafka** (`Confluent.Kafka 2.6.1`) - Mensageria para Event-Driven Architecture
- **MediatR** (`13.1.0`) - Mediator pattern para CQRS

### Persistência
- **Entity Framework Core 9.0** - ORM
- **SQL Server 2022** - Banco de dados relacional
- **Migrations** - Controle de versionamento do schema

### Segurança
- **JWT Bearer Authentication** (`Microsoft.AspNetCore.Authentication.JwtBearer 8.0.22`)
- **BCrypt.Net** (`1.6.0`) - Hashing de senhas

### Validação e Qualidade
- **FluentValidation** (`12.1.0`) - Validação de objetos
- **Serilog** (`4.3.0`) - Logging estruturado
- **Seq** - Centralização de logs

### API e Documentação
- **Swagger/OpenAPI** (`Swashbuckle.AspNetCore 6.6.2`)
- **API Versioning** (`Asp.Versioning.Http 8.1.0`)

### Observabilidade
- **Health Checks** - Monitoramento de saúde da aplicação
- **Correlation ID** - Rastreamento de requisições

### Testes
- **xUnit** - Framework de testes
- **SpecFlow/Reqnroll** - Testes funcionais (BDD)
- **FluentAssertions** - Assertions fluentes

---

## 💾 Modelo de Dados

### Tabela `Users`

```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    Password NVARCHAR(512) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT CK_User_Role CHECK (Role IN ('User', 'Admin')),
    INDEX IX_Users_Email (Email)
);
```

**Campos:**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | UNIQUEIDENTIFIER | Identificador único (GUID) |
| `Name` | NVARCHAR(100) | Nome completo do usuário |
| `Email` | NVARCHAR(256) | Email único (usado para login) |
| `Password` | NVARCHAR(512) | Hash da senha (BCrypt) |
| `Role` | NVARCHAR(20) | Perfil de acesso: `User` ou `Admin` |
| `IsActive` | BIT | Indica se usuário está ativo |
| `CreatedAt` | DATETIME2 | Data/hora de criação |
| `UpdatedAt` | DATETIME2 | Data/hora da última atualização |

### Tabela `RefreshTokens`

```sql
CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Token NVARCHAR(512) NOT NULL UNIQUE,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    RevokedAt DATETIME2 NULL,
    IsRevoked BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX IX_RefreshTokens_UserId (UserId),
    INDEX IX_RefreshTokens_Token (Token)
);
```

**Campos:**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | UNIQUEIDENTIFIER | Identificador único do token |
| `UserId` | UNIQUEIDENTIFIER | Referência ao usuário (FK) |
| `Token` | NVARCHAR(512) | String do refresh token (hash) |
| `ExpiresAt` | DATETIME2 | Data/hora de expiração (7 dias) |
| `CreatedAt` | DATETIME2 | Data/hora de criação |
| `RevokedAt` | DATETIME2 | Data/hora de revogação (nullable) |
| `IsRevoked` | BIT | Flag indicando se foi revogado |

---

## 📐 Regras de Negócio

### RN-USER-001: Cadastro de Usuário
- ✅ Email deve ser único no sistema
- ✅ Senha deve ter no mínimo 8 caracteres (letra + número + caractere especial)
- ✅ Nome deve ter entre 2 e 250 caracteres
- ✅ Todo novo usuário tem `Role = 'User'` por padrão
- ✅ Email deve ser validado (formato válido)
- ✅ Ao criar usuário, disparar evento `UserCreatedEvent`

### RN-USER-002: Autenticação (Login)
- ✅ Validar credenciais (email + senha)
- ✅ Gerar **JWT Access Token** com validade de **60 minutos**
- ✅ Gerar **Refresh Token** com validade de **7 dias**
- ✅ Access Token contém claims: `UserId`, `Email`, `Role`
- ✅ Refresh Token armazenado no banco

### RN-USER-003: Refresh Token
- ✅ Validar se Refresh Token existe e não expirou
- ✅ Validar se não foi revogado (`IsRevoked = false`)
- ✅ Gerar novo Access Token e novo Refresh Token
- ✅ Revogar Refresh Token anterior (`IsRevoked = true`)

### RN-USER-004: Atualização de Senha
- ✅ Usuário só pode atualizar sua própria senha
- ✅ Validar senha atual antes de permitir alteração
- ✅ Nova senha deve seguir política de senha forte

### RN-USER-005: Gestão de Roles (Admin)
- ✅ Apenas usuários com `Role = 'Admin'` podem alterar roles
- ✅ Admin não pode alterar sua própria role
- ✅ Roles permitidas: `User`, `Admin`

### RN-USER-006: Listagem de Usuários (Admin)
- ✅ Apenas Admin pode listar todos os usuários
- ✅ Implementar paginação (`pageNumber`, `pageSize`)

### RN-USER-007: Cadastro de Usuário (Admin)
- ✅ Apenas Admin pode cadastrar usuário escolhendo a role

---

## 🔌 Endpoints da API

### Autenticação

| Método | Endpoint | Autenticação | Autorização | Descrição |
|--------|----------|--------------|-------------|-----------|
| `POST` | `/v1/auth/login` | ❌ Não | Público | Autenticar usuário |
| `POST` | `/v1/auth/refresh-token` | ❌ Não | Público | Renovar Access Token |

**POST /v1/auth/login**
```json
Request:
{
  "email": "user@example.com",
  "password": "MyP@ssw0rd"
}

Response:
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "a1b2c3d4e5f6...",
  "expiresIn": 3600
}
```

**POST /v1/auth/refresh-token**
```json
Request:
{
  "refreshToken": "a1b2c3d4e5f6..."
}

Response:
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "g7h8i9j0k1l2...",
  "expiresIn": 3600
}
```

### Usuários

| Método | Endpoint | Autenticação | Autorização | Descrição |
|--------|----------|--------------|-------------|-----------|
| `POST` | `/v1/users` | ❌ Não | Público | Cadastrar novo usuário |
| `PUT` | `/v1/users/update-password` | ✅ Sim | User (próprio) | Atualizar senha |

**POST /v1/users**
```json
Request:
{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "MyP@ssw0rd"
}

Response: 201 Created
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva",
  "email": "joao@example.com",
  "role": "User",
  "createdAt": "2026-01-18T10:30:00Z"
}
```

**PUT /v1/users/update-password**
```json
Request:
{
  "currentPassword": "MyP@ssw0rd",
  "newPassword": "NewP@ssw0rd123"
}

Response: 204 No Content
```

### Administração

| Método | Endpoint | Autenticação | Autorização | Descrição |
|--------|----------|--------------|-------------|-----------|
| `PATCH` | `/v1/admin/users/{id}/update-role` | ✅ Sim | Admin | Alterar role de usuário |
| `GET` | `/v1/admin/users` | ✅ Sim | Admin | Listar usuários (paginado) |
| `POST` | `/v1/admin/users` | ✅ Sim | Admin | Criar usuário com role |

**GET /v1/admin/users?pageNumber=1&pageSize=10**
```json
Response:
{
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "João Silva",
      "email": "joao@example.com",
      "role": "User",
      "isActive": true,
      "createdAt": "2026-01-18T10:30:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "totalRecords": 50
}
```

**PATCH /v1/admin/users/{id}/update-role**
```json
Request:
{
  "role": "Admin"
}

Response: 204 No Content
```

---

## 📤 Eventos

A aplicação publica eventos de domínio via **Apache Kafka** para comunicação assíncrona com outros microserviços.

### UserCreatedEvent

**Tópico Kafka:** `user-created`

```json
{
  "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f",
  "name": "João Silva",
  "email": "joao@example.com",
  "createdAt": "2026-01-18T10:30:00Z"
}
```

**Quando é disparado:**
- ✅ Ao cadastrar novo usuário via `POST /v1/users`
- ✅ Ao admin criar usuário via `POST /v1/admin/users`

---

## ⚙️ Configuração de Ambiente

### Variáveis e Secrets Necessários

| Variável | Descrição | Obrigatório | Exemplo |
|----------|-----------|:-----------:|---------|
| `ConnectionStrings:DefaultConnection` | Connection string do SQL Server | ✅ Sim | `Server=localhost;Database=fcg_user;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;` |
| `JwtSettings:SecretKey` | Chave secreta para assinatura JWT | ✅ Sim | `chave-base64-com-minimo-32-caracteres` |
| `KafkaSettings:SaslUsername` | Usuário SASL do Kafka (produção) | ⚠️ Produção | `$ConnectionString` |
| `KafkaSettings:SaslPassword` | Senha SASL do Kafka (produção) | ⚠️ Produção | `Endpoint=sb:...` |

### Pré-requisitos

- .NET 8 SDK
- Docker e Docker Compose
- SQL Server 2022
- Apache Kafka (via Docker)

### Configuração Local (user-secrets)

```bash
cd src/FCG.Users.WebApi

# Inicializar user-secrets (já feito se o .csproj contém UserSecretsId)
dotnet user-secrets init

# Configurar os secrets obrigatórios
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1;Database=fcg_user;User Id=sa;Password=SuaSenhaForte123;TrustServerCertificate=True;"
dotnet user-secrets set "JwtSettings:SecretKey" "sua-chave-secreta-jwt-com-minimo-32-caracteres"

# Secrets opcionais (Kafka SASL - apenas para ambiente com Event Hubs)
dotnet user-secrets set "KafkaSettings:SaslUsername" "$ConnectionString"
dotnet user-secrets set "KafkaSettings:SaslPassword" "Endpoint=sb:..."

# Verificar secrets configurados
dotnet user-secrets list
```

### Execução via Docker

1. Copie o arquivo `.env.example` para `.env`:
   ```bash
   cp .env.example .env
   ```

2. Preencha as variáveis no `.env`:
   ```env
   SA_PASSWORD=SuaSenhaForte123
   JWT_SECRET_KEY=sua-chave-secreta-jwt-com-minimo-32-caracteres
   SEQ_ADMIN_PASSWORD=SenhaDoSeq123
   ```

3. Suba os serviços:
   ```bash
   docker-compose up -d
   ```

Serviços disponíveis:
- SQL Server: `localhost:1433`
- Seq (Logs): `http://localhost:5341`
- Kafka: `localhost:9092`
- Kafka UI: `http://localhost:8081`

### Arquivos que NUNCA devem ser commitados

| Arquivo | Motivo |
|---------|--------|
| `appsettings.Development.json` | Pode conter secrets locais |
| `appsettings.Production.json` | Contém configurações de produção |
| `appsettings.Docker.json` | Contém configurações de infraestrutura |
| `.env` | Contém senhas e chaves reais |
| `secrets.json` | Arquivo de secrets do .NET |

Esses arquivos já estão no `.gitignore` do repositório.

---

## 📊 Auditoria

A aplicação implementa um sistema de auditoria automático que rastreia todas as mudanças nas entidades auditáveis.

### Tabela `AuditTrails`
```sql
CREATE TABLE audit_trails ( 
    Id UNIQUEIDENTIFIER PRIMARY KEY, 
    UserId UNIQUEIDENTIFIER NULL, 
    EntityName NVARCHAR(100) NOT NULL, 
    PrimaryKey NVARCHAR(100) NOT NULL, 
    TrailType NVARCHAR(20) NOT NULL, 
    DateUtc DATETIMEOFFSET NOT NULL, 
    OldValues NVARCHAR(MAX), 
    NewValues NVARCHAR(MAX), 
    ChangedColumns NVARCHAR(MAX),
    INDEX IX_AuditTrails_EntityName (EntityName)
);
```

**Campos:**

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | UNIQUEIDENTIFIER | Identificador único do registro de auditoria |
| `UserId` | UNIQUEIDENTIFIER (NULL) | ID do usuário que realizou a ação |
| `EntityName` | NVARCHAR(100) | Nome da entidade modificada (ex: User) |
| `PrimaryKey` | NVARCHAR(100) | ID (PK) da entidade modificada |
| `TrailType` | NVARCHAR(20) | Tipo de operação: Create, Update ou Delete |
| `DateUtc` | DATETIMEOFFSET | Data/hora (UTC) da operação |
| `OldValues` | NVARCHAR(MAX) | JSON com valores anteriores (atualização/exclusão) |
| `NewValues` | NVARCHAR(MAX) | JSON com valores novos (criação/atualização) |
| `ChangedColumns` | NVARCHAR(MAX) | JSON com lista de colunas modificadas |

### Como Acessar os Registros de Auditoria

#### 1️º Listar todos os registros de auditoria
```sql
SELECT * FROM dbo.audit_trails ORDER BY DateUtc DESC;
```

#### 2️º Filtrar por entidade
```sql
SELECT * FROM dbo.audit_trails WHERE EntityName = 'User' ORDER BY DateUtc DESC;
```

#### 3️º Filtrar por usuário
```sql
SELECT * FROM dbo.audit_trails WHERE UserId = '7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f' ORDER BY DateUtc DESC;
```

#### 4️º Filtrar por tipo de operação
```sql
SELECT * FROM dbo.audit_trails WHERE TrailType = 'Update' AND DateUtc >= GETUTCDATE() - 1 ORDER BY DateUtc DESC;
```

#### 5️º Analisar mudanças de um usuário específico
```sql
SELECT PrimaryKey, TrailType, DateUtc, CAST(OldValues AS NVARCHAR(MAX)) AS OldValues, CAST(NewValues AS NVARCHAR(MAX)) AS NewValues, CAST(ChangedColumns AS NVARCHAR(MAX)) AS ChangedColumns FROM dbo.audit_trails WHERE EntityName = 'User' AND PrimaryKey = '3fa85f64-5717-4562-b3fc-2c963f66afa6' ORDER BY DateUtc DESC
```

### Exemplo de Dados em Auditoria

**Atualização de Senha:**
```
{ "Id": "a1b2c3d4-e5f6-4a5b-9c8d-7e6f5a4b3c2d", "UserId": "7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f", "EntityName": "User", "PrimaryKey": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "TrailType": "Update", "DateUtc": "2026-01-18T15:45:30.1234567Z", "OldValues": "{"Password":"$2a$11$OldHashedPassword..."}", "NewValues": "{"Password":"$2a$11$NewHashedPassword...","UpdatedAt":"2026-01-18T15:45:30.1234567Z"}", "ChangedColumns": "["Password","UpdatedAt"]" }
```

**Interpretação:**
- Usuário 7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f atualizou a senha
- Entidade User com ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 foi modificada
- Colunas alteradas: Password e UpdatedAt
- Timestamp UTC de quando a ação ocorreu

### Entidades Auditadas

Apenas entidades que implementam `IAuditableEntity` são auditadas:

| Entidade     | Auditada |
|--------------|----------|
| User         |  ✅ Sim  |
| RefreshToken |  ❌ Não  |

**Para adicionar auditoria a uma nova entidade:**
```
public sealed class MinhaEntidade : BaseEntity, IAuditableEntity { 
    // ... propriedades

    // Implementação explícita da interface
    DateTime IAuditableEntity.CreatedAt { get => CreatedAt; set => CreatedAt = value; }
    DateTime? IAuditableEntity.UpdatedAt { get => UpdatedAt; set => UpdatedAt = value; }
    string IAuditableEntity.CreatedBy { get => CreatedBy; set => CreatedBy = value; }
    string? IAuditableEntity.UpdatedBy { get => UpdatedBy; set => UpdatedBy = value; }
}
```

## 📚 Referências
- [Artigo](https://blog.elmah.io/implementing-audit-logs-in-ef-core-without-polluting-your-entities/)
---
