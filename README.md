# Controle de Gastos com Apostas (Console + EF Core)

Aplicação de console em C# para controlar gastos com apostas com CRUD completo, persistência local usando Entity Framework Core e SQLite.

## Integrantes da Equipe

| **Nome**                   | **RM**   |
|-----------------------------|----------|
| Rodrigo Fernandes Serafim  | RM550816 |
| João Antonio Rihan         | RM99656  |
| Adriano Lopes              | RM98574  |
| Henrique de Brito          | RM98831  |
| Rodrigo Lima               | RM98326  |

## Descrição

Esse projeto permite que o usuário gerencie gastos com apostas via terminal, com operações de **listar**, **adicionar**, **editar**, **remover** apostas e **relatório de gastos**. As apostas têm os campos:

- Id  
- Nome da Casa de Aposta  
- Valor Apostado (R$)  
- Resultado da Aposta (Ganhou, Perdeu, Pendente)
- Descrição (opcional)

A persistência é feita por meio do **Entity Framework Core** usando **SQLite** em arquivo local. Migrações são aplicadas automaticamente no início da aplicação.

## Tecnologias e ferramentas
- **.NET**: 9.0 (Console App)
- **Entity Framework Core**: 9.0
  - Provider: `Microsoft.EntityFrameworkCore.Sqlite`
  - Design-time: `Microsoft.EntityFrameworkCore.Design`
- **SQLite**: banco de dados local em arquivo
- **dotnet-ef**: ferramenta de linha de comando para migrations

## Como executar
1. Requisitos: .NET SDK 9 instalado
2. No diretório do projeto, execute:
```bash
# Restaurar e compilar
dotnet build .\BettingTracker.Console\BettingTracker.Console.csproj

# Rodar a aplicação de console
dotnet run --project .\BettingTracker.Console\BettingTracker.Console.csproj
```
Ao iniciar, o app aplica as migrations pendentes e mostra o menu no console.

## Estrutura de pastas (resumo)
```
BettingTracker.Console/
  Data/
    Db.cs                      # Contexto EF Core (ContextoDados)
    DbFactory.cs               # Fábrica design-time para dotnet-ef
    Migrations/                # Migrations geradas pelo EF
  Models/
    Aposta.cs                  # Entidade Aposta
  Repositories/
    ApostaRepository.cs        # ApostaRepositorio (CRUD)
  Program.cs                   # Menu do console e fluxo da aplicação
bettingtracker.db               # Banco de dados SQLite (gerado em runtime)
```

## Fluxo do console (CRUD + Relatórios)
1) Listar apostas
2) Adicionar aposta
3) Editar aposta
4) Remover aposta
5) Relatório de gastos
0) Sair

## Funcionalidades do Relatório
- Total apostado
- Total ganho
- Total perdido
- Saldo (ganho - perdido)
- Status: LUCRO, PREJUÍZO ou EMPATE

## Validações básicas:
- Nome da casa de apostas é obrigatório (até 200 caracteres)
- Valor apostado deve ser maior que zero
- Resultado deve ser: Ganhou, Perdeu ou Pendente
- Descrição é opcional (até 1000 caracteres)

---
## Arquitetura em Camadas
flowchart TD:

- A[Console / Program.cs] --> B[Domínio / Models]
- B --> C[Repositórios / Repository]
- C --> D[(Banco SQLite)]
- C --> E[DbContext (EF Core)]

---
## Diagrama de Classes (simplificado)
```
classDiagram
    class Aposta {
        int Id
        string NomeCasaAposta
        decimal ValorApostado
        ResultadoAposta Resultado
        string Descricao
        +Validar()
    }

    class ApostaRepository {
        +Listar() IEnumerable<Aposta>
        +ObterPorId(int id) Aposta
        +Adicionar(Aposta aposta)
        +Atualizar(Aposta aposta)
        +Remover(int id)
    }

    class ContextoDados {
        DbSet<Aposta> Apostas
        +SaveChanges()
    }

    class Program {
        +Main()
        +MostrarMenu()
        +ExecutarOpcao(int opcao)
    }

    ApostaRepository --> ContextoDados
    Program --> ApostaRepository
    Program --> Aposta
```

---

## Diagrama de Sequência
```
sequenceDiagram
    participant U as Usuário
    participant P as Program (Console)
    participant R as ApostaRepository
    participant D as DbContext/SQLite

    U->>P: Seleciona "Adicionar Aposta"
    P->>U: Solicita dados (casa, valor, resultado, descrição)
    U->>P: Informa dados
    P->>P: Cria objeto Aposta
    P->>R: Adicionar(aposta)
    R->>D: INSERT na tabela Apostas
    D-->>R: Confirmação
    R-->>P: Sucesso
    P->>U: "Aposta adicionada com sucesso"
```

