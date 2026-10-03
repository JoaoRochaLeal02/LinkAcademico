using System;
using System.ComponentModel.DataAnnotations;

namespace LinkAcademico.Models
{
    public class CandidaturaVaga
    {
        public int Id { get; set; }

        public int VagaId { get; set; }

        public Vaga? Vaga { get; set; }

        public int EstudanteId { get; set; }

        public Estudante? Estudante { get; set; }

        public DateTime DataCandidatura { get; set; } = DateTime.Now;

        [Display(Name = "Mensagem para a empresa")]
        [MaxLength(800, ErrorMessage = "A mensagem pode ter no máximo 800 caracteres.")]
        public string? Mensagem { get; set; }
    }
}
