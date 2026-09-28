# OrdersBackend

Backend de gestão de pedidos para um e-commerce simples, construído em .NET 10 com Clean Architecture, CQRS (MediatR), EF Core + SQLite e autenticação JWT.

## Sumário

- [Como rodar](#como-rodar)
  - [Pré-requisitos](#pré-requisitos)
  - [Local](#local)
  - [Docker](#docker)
- [Autenticação](#autenticação)
- [Endpoints](#endpoints)
  - [Erros](#erros)
- [Testes](#testes)
- [Arquitetura](#arquitetura)
- [Decisões técnicas](#decisões-técnicas)
  - [Minimal API em vez de Controllers](#minimal-api-em-vez-de-controllers)

## Como rodar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) para rodar localmente
- [Docker](https://www.docker.com/) para rodar via container

O banco é criado e as migrations são aplicadas automaticamente na inicialização. Não é necessário nenhum passo manual.

### Local

```bash
dotnet run --project src/OrdersBackend.Api
```

Swagger: http://localhost:5231/swagger

O banco SQLite é criado em `src/OrdersBackend.Api/orders.db`.

### Docker

```bash
docker compose up --build
```

Swagger: http://localhost:8080/swagger

O banco fica no volume `orders-data`, então os dados persistem entre reinicializações do container. Para começar com o banco vazio:

```bash
docker compose down -v
```

O container expõe apenas HTTP.

## Autenticação

Todos os endpoints de pedidos exigem um token JWT. O login usa um usuário fixo em memória:

| E-mail | Senha |
|---|---|
| `dev@martech.com` | `Senha@123` |

**No Swagger:**

1. Chame `POST /auth/login` com o usuário acima.
2. Copie o `accessToken` da resposta.
3. Clique em **Authorize** e cole o token (sem o prefixo `Bearer`).

## Endpoints

Os contratos de request e response estão documentados no Swagger: http://localhost:5231/swagger (local) ou http://localhost:8080/swagger (Docker).

| Método | Rota | Descrição | Sucesso |
|---|---|---|---|
| `POST` | `/auth/login` | Retorna um JWT | `200` |
| `POST` | `/api/orders` | Cria um pedido | `201` + `Location` |
| `GET` | `/api/orders?page=1&pageSize=10` | Lista pedidos paginados, do mais recente para o mais antigo | `200` |
| `GET` | `/api/orders/{id}` | Retorna um pedido | `200` |
| `PATCH` | `/api/orders/{id}/cancel` | Cancela um pedido pendente | `204` |

### Erros

Todas as respostas de erro seguem o formato [ProblemDetails (RFC 9457)](https://www.rfc-editor.org/rfc/rfc9457).

| Status | Quando |
|---|---|
| `400` | Dados inválidos ou corpo da requisição malformado. Os erros vêm agrupados por campo. |
| `401` | Token ausente ou inválido, ou credenciais de login incorretas |
| `404` | Pedido não encontrado |
| `422` | Regra de negócio violada, por exemplo cancelar um pedido que não está pendente |

## Testes

Testes unitários com xUnit.

```bash
dotnet test
```

## Arquitetura

```
src/
  OrdersBackend.Domain/          Entidades e regras de negócio
  OrdersBackend.Application/     Casos de uso (commands e queries)
  OrdersBackend.Infrastructure/  Persistência e autenticação
  OrdersBackend.Api/             Endpoints HTTP
tests/
  OrdersBackend.Domain.Tests/
  OrdersBackend.Application.Tests/
```

## Decisões técnicas

### Minimal API em vez de Controllers

Com MediatR, o endpoint só converte a requisição em command ou query e devolve o resultado.
