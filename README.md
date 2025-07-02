# API de Transferências Bancárias - Case Itaú

Projeto desenvolvido em .NET 8 que simula transferências entre contas bancárias.

---
## Requisitos para executar localmente

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022 (preferencial) ou Visual Studio Code
---

## Como rodar a aplicação

1. Clone este repositório:

```bash
git clone https://github.com/seu-usuario/nome-do-repo.git
cd nome-do-repo
```

2. Restaure os pacotes e compile a solução:

```bash
dotnet restore
dotnet build
```

3. Execute a aplicação:

```bash
dotnet run --project Case.TransferenciaAPI
```

4. Acesse a documentação Swagger no navegador:

```
http://localhost:5000/swagger
```

> A porta pode variar — verifique o terminal ou console do Visual Studio para confirmar.

---

## Dados iniciais carregados

Ao iniciar a aplicação, são criadas 3 contas automaticamente no banco de dados em memória:

| Nome   | Conta     | Saldo  |
|--------|-----------|--------|
| João   | 11111-1   | R$1000 |
| Maria  | 22222-1   | R$500  |
| Carlos | 33333-1   | R$300  |

---

## Como rodar os testes

Execute os testes com o seguinte comando:

```bash
dotnet test
```

## Endpoints principais

| Método | Rota                          | Descrição                           |
|--------|-------------------------------|-------------------------------------|
| POST   | /api/v1/clientes              | Cria um novo cliente                |
| GET    | /api/v1/clientes              | Lista todos os clientes             |
| POST   | /api/v1/transferencias        | Realiza uma transferência bancária  |
| GET    | /api/v1/transferencias        | Lista o histórico de transferências |
| GET    | /api/v1/transferencias/historico/{NumeroConta}   | Consulta transferências por numero da conta |

---
