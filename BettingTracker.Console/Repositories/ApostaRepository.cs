using Microsoft.EntityFrameworkCore;
using BettingTracker.ConsoleApp.Data;
using BettingTracker.ConsoleApp.Models;

namespace BettingTracker.ConsoleApp.Repositories
{
    // Repositório com operações de CRUD para a entidade Aposta.
    // Centraliza o acesso ao banco para manter o Program.cs mais limpo.
    public class ApostaRepositorio
    {
        private readonly ContextoDados _dbContext;

        public ApostaRepositorio(ContextoDados dbContext)
        {
            _dbContext = dbContext;
        }

        // Cria uma nova aposta
        public async Task<Aposta> CriarAsync(Aposta aposta)
        {
            _dbContext.Apostas.Add(aposta);
            await _dbContext.SaveChangesAsync();
            return aposta;
        }

        // Busca uma aposta pelo Id
        public async Task<Aposta?> ObterPorIdAsync(int id)
        {
            return await _dbContext.Apostas.FindAsync(id);
        }

        // Lista todas as apostas, da mais recente para a mais antiga
        public async Task<List<Aposta>> ObterTodosAsync()
        {
            return await _dbContext.Apostas
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();
        }

        // Atualiza os dados de uma aposta existente
        public async Task<bool> AtualizarAsync(Aposta aposta)
        {
            var exists = await _dbContext.Apostas.AnyAsync(a => a.Id == aposta.Id);
            if (!exists) return false;

            _dbContext.Apostas.Update(aposta);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Remove uma aposta pelo Id
        public async Task<bool> RemoverAsync(int id)
        {
            var entity = await _dbContext.Apostas.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Apostas.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Calcula o total apostado
        public async Task<decimal> ObterTotalApostadoAsync()
        {
            return await _dbContext.Apostas.SumAsync(a => a.ValorApostado);
        }

        // Calcula o total de apostas ganhas
        public async Task<decimal> ObterTotalGanhoAsync()
        {
            return await _dbContext.Apostas
                .Where(a => a.ResultadoAposta.ToLower() == "ganhou")
                .SumAsync(a => a.ValorApostado);
        }

        // Calcula o total de apostas perdidas
        public async Task<decimal> ObterTotalPerdidoAsync()
        {
            return await _dbContext.Apostas
                .Where(a => a.ResultadoAposta.ToLower() == "perdeu")
                .SumAsync(a => a.ValorApostado);
        }
    }
}


