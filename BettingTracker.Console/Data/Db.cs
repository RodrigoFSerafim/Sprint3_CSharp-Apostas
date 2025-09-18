using Microsoft.EntityFrameworkCore;
using BettingTracker.ConsoleApp.Models;

namespace BettingTracker.ConsoleApp.Data
{
    // Contexto de dados do Entity Framework Core.
    // Ele representa a conexão com o banco e o mapeamento das entidades.
    public class ContextoDados : DbContext
    {
        // Conjunto de Apostas (tabela) no banco de dados
        public DbSet<Aposta> Apostas => Set<Aposta>();

        // Construtor recebendo as opções (ex.: provedor SQLite)
        public ContextoDados(DbContextOptions<ContextoDados> options) : base(options)
        {
        }

        // Configurações de mapeamento das entidades
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aposta>(entity =>
            {
                // Nome da tabela para apostas
                entity.ToTable("Apostas");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.NomeCasaAposta).IsRequired().HasMaxLength(200);
                entity.Property(a => a.ValorApostado).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(a => a.ResultadoAposta).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Descricao).HasMaxLength(1000);
                entity.Property(a => a.CreatedAtUtc).IsRequired();
            });
        }
    }
}


