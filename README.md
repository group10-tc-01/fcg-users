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

## ⚙️ Configuração e Execução

### Pré-requisitos

- ✅ .NET 8 SDK
- ✅ Docker e Docker Compose
- ✅ SQL Server 2022
- ✅ Apache Kafka (via Docker)

### Configuração de Ambiente

**appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=fcg_user;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-min-32-chars",
    "Issuer": "FCG.Users.API",
    "Audience": "FCG.Users.Client",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  },
  "KafkaSettings": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "fcg-users-api"
  }
}
```

### Executar com Docker Compose

```bash
docker-compose up -d
```

Serviços disponíveis:
- 🐳 **SQL Server**: `localhost:1433`
- 📊 **Seq (Logs)**: `http://localhost:5341`
- 📨 **Kafka**: `localhost:9092`
- 🎛️ **Kafka UI**: `http://localhost:8080`