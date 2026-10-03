using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkAcademico.Models
{
    public class Empresa
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string AreaAtuacao { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public string? Tecnologias { get; set; }

        public string? OQueBusca { get; set; }

        [Display(Name = "Egresso empreendedor")]
        public bool EhEgressoEmpreendedor { get; set; }

        public string? FotoPerfil { get; set; }

        [NotMapped]
        public IFormFile? FotoUpload { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public List<Vaga> Vagas { get; set; }
            = new List<Vaga>();
    }
}
