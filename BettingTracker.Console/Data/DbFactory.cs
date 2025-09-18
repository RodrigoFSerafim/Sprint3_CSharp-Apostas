using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BettingTracker.ConsoleApp.Data
{
    // Fábrica usada em tempo de design (design-time) para gerar migrations
    // e executar comandos do EF Core sem precisar rodar a aplicação.
    public class FabricaContextoDados : IDesignTimeDbContextFactory<ContextoDados>
    {
        public ContextoDados CreateDbContext(string[] args)
        {
            // Define o provedor e a string de conexão (SQLite em arquivo local)
            var optionsBuilder = new DbContextOptionsBuilder<ContextoDados>();
            optionsBuilder.UseSqlite("Data Source=bettingtracker.db");
            // Cria uma instância do contexto com essas opções
            return new ContextoDados(optionsBuilder.Options);
        }
    }
}


