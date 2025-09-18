using System;
using System.ComponentModel.DataAnnotations;

namespace BettingTracker.ConsoleApp.Models
{
    // Representa uma aposta feita pelo usuário
    public class Aposta
    {
        // Identificador único no banco de dados
        public int Id { get; set; }

        // Nome da casa de apostas
        [Required]
        [MaxLength(200)]
        public string NomeCasaAposta { get; set; } = string.Empty;

        // Valor apostado
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal ValorApostado { get; set; }

        // Resultado da aposta (Ganhou, Perdeu, Pendente)
        [Required]
        [MaxLength(50)]
        public string ResultadoAposta { get; set; } = string.Empty;

        // Descrição da aposta (opcional)
        [MaxLength(1000)]
        public string? Descricao { get; set; }

        // Momento em que a aposta foi registrada (UTC)
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}


