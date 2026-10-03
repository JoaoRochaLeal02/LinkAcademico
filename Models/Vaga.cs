using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinkAcademico.Models
{
    public class Vaga
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Os requisitos são obrigatórios.")]
        [Display(Name = "Requisitos")]
        public string Requisitos { get; set; } = string.Empty;

        [Display(Name = "Local")]
        public string? Local { get; set; }

        [Display(Name = "Modelo de trabalho")]
        public string? ModeloTrabalho { get; set; }

        [Display(Name = "Tipo de vaga")]
        public string? TipoVaga { get; set; }

        [Display(Name = "Bolsa/Salário")]
        public string? Remuneracao { get; set; }

        [Display(Name = "Data limite")]
        [DataType(DataType.Date)]
        public DateTime? DataLimite { get; set; }

        public DateTime DataPublicacao { get; set; } = DateTime.Now;

        public bool Ativa { get; set; } = true;

        public int EmpresaId { get; set; }

        public Empresa? Empresa { get; set; }

        public List<CandidaturaVaga> Candidaturas { get; set; }
            = new List<CandidaturaVaga>();
    }
}
