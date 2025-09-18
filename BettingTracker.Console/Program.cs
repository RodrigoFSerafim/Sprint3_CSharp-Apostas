using Microsoft.EntityFrameworkCore;
using BettingTracker.ConsoleApp.Data;
using BettingTracker.ConsoleApp.Models;
using BettingTracker.ConsoleApp.Repositories;
 

// Configura o EF Core para usar SQLite e abre a conexão com o banco
var options = new DbContextOptionsBuilder<ContextoDados>()
    .UseSqlite("Data Source=bettingtracker.db")
    .Options;

// Garante que o banco/migrations estejam aplicados ao iniciar o app
await using var db = new ContextoDados(options);
await db.Database.MigrateAsync();

// Repositório para manipular as apostas
var repo = new ApostaRepositorio(db);

// Loop principal do menu do console
bool exit = false;
while (!exit)
{
    Console.WriteLine();
    Console.WriteLine("==== Controle de Gastos com Apostas ====");
    Console.WriteLine("1) Listar apostas");
    Console.WriteLine("2) Adicionar aposta");
    Console.WriteLine("3) Editar aposta");
    Console.WriteLine("4) Remover aposta");
    Console.WriteLine("5) Relatório de gastos");
    
    Console.WriteLine("0) Sair");
    Console.Write("Escolha: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await ListarAsync(repo);
            break;
        case "2":
            await AdicionarAsync(repo);
            break;
        case "3":
            await EditarAsync(repo);
            break;
        case "4":
            await RemoverAsync(repo);
            break;
        case "5":
            await RelatorioAsync(repo);
            break;
        case "0":
            exit = true;
            break;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }
}

// Mostra todas as apostas cadastradas
static async Task ListarAsync(ApostaRepositorio repo)
{
    var items = await repo.ObterTodosAsync();
    if (items.Count == 0)
    {
        Console.WriteLine("Nenhuma aposta encontrada.");
        return;
    }
    foreach (var a in items)
    {
        Console.WriteLine($"#{a.Id} - {a.NomeCasaAposta} | R$ {a.ValorApostado:F2} | {a.ResultadoAposta} | {a.CreatedAtUtc:dd/MM/yyyy HH:mm}");
        if (!string.IsNullOrWhiteSpace(a.Descricao))
        {
            Console.WriteLine($"  {a.Descricao}");
        }
    }
}

// Solicita os dados ao usuário e cria uma nova aposta
static async Task AdicionarAsync(ApostaRepositorio repo)
{
    Console.Write("Nome da casa de apostas: ");
    var nomeCasa = Console.ReadLine()?.Trim() ?? string.Empty;

    decimal valor = ReadValor();

    string resultado = ReadResultado();

    Console.Write("Descrição (opcional): ");
    var desc = Console.ReadLine();

    var aposta = new Aposta 
    { 
        NomeCasaAposta = nomeCasa, 
        ValorApostado = valor, 
        ResultadoAposta = resultado, 
        Descricao = desc 
    };
    await repo.CriarAsync(aposta);
    Console.WriteLine("Aposta adicionada com sucesso.");
}

// Edita os dados de uma aposta existente
static async Task EditarAsync(ApostaRepositorio repo)
{
    Console.Write("Id da aposta para editar: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Id inválido.");
        return;
    }
    var existing = await repo.ObterPorIdAsync(id);
    if (existing == null)
    {
        Console.WriteLine("Aposta não encontrada.");
        return;
    }

    Console.Write($"Nome da casa de apostas ({existing.NomeCasaAposta}): ");
    var nomeCasa = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(nomeCasa)) existing.NomeCasaAposta = nomeCasa.Trim();

    Console.Write($"Valor apostado (R$ {existing.ValorApostado:F2}): ");
    var valorInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(valorInput) && decimal.TryParse(valorInput, out var newValor))
    {
        if (newValor <= 0)
        {
            Console.WriteLine("Valor deve ser maior que zero. Mantendo o valor antigo.");
        }
        else
        {
            existing.ValorApostado = newValor;
        }
    }

    Console.Write($"Resultado ({existing.ResultadoAposta}): ");
    var resultado = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(resultado)) existing.ResultadoAposta = resultado.Trim();

    Console.Write($"Descrição ({existing.Descricao}): ");
    var desc = Console.ReadLine();
    if (desc != null) existing.Descricao = desc;

    var ok = await repo.AtualizarAsync(existing);
    Console.WriteLine(ok ? "Atualizado." : "Falha ao atualizar.");
}

// Remove uma aposta a partir do Id
static async Task RemoverAsync(ApostaRepositorio repo)
{
    Console.Write("Id da aposta para remover: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Id inválido.");
        return;
    }
    var ok = await repo.RemoverAsync(id);
    Console.WriteLine(ok ? "Removido." : "Aposta não encontrada.");
}

// Mostra relatório de gastos
static async Task RelatorioAsync(ApostaRepositorio repo)
{
    var totalApostado = await repo.ObterTotalApostadoAsync();
    var totalGanho = await repo.ObterTotalGanhoAsync();
    var totalPerdido = await repo.ObterTotalPerdidoAsync();
    var saldo = totalGanho - totalPerdido;

    Console.WriteLine();
    Console.WriteLine("==== RELATÓRIO DE GASTOS ====");
    Console.WriteLine($"Total apostado: R$ {totalApostado:F2}");
    Console.WriteLine($"Total ganho: R$ {totalGanho:F2}");
    Console.WriteLine($"Total perdido: R$ {totalPerdido:F2}");
    Console.WriteLine($"Saldo: R$ {saldo:F2}");
    
    if (saldo > 0)
        Console.WriteLine("Status: LUCRO");
    else if (saldo < 0)
        Console.WriteLine("Status: PREJUÍZO");
    else
        Console.WriteLine("Status: EMPATE");
}

// Lê e valida o valor apostado
static decimal ReadValor()
{
    while (true)
    {
        Console.Write("Valor apostado (R$): ");
        var valorStr = Console.ReadLine();
        if (decimal.TryParse(valorStr, out var valor) && valor > 0)
        {
            return valor;
        }
        Console.WriteLine("Valor inválido. Digite um valor maior que zero.");
    }
}

// Lê e valida o resultado da aposta
static string ReadResultado()
{
    while (true)
    {
        Console.WriteLine("Resultado da aposta:");
        Console.WriteLine("1) Ganhou");
        Console.WriteLine("2) Perdeu");
        Console.WriteLine("3) Pendente");
        Console.Write("Escolha (1-3): ");
        var choice = Console.ReadLine();
        
        switch (choice)
        {
            case "1":
                return "Ganhou";
            case "2":
                return "Perdeu";
            case "3":
                return "Pendente";
            default:
                Console.WriteLine("Opção inválida. Tente novamente.");
                break;
        }
    }
}
