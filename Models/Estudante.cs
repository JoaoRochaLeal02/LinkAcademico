using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace LinkAcademico.Models
{
    public class Estudante
    {
        public int Id { get; set; }

        // OBRIGATÓRIOS

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O curso é obrigatório.")]
        [Display(Name = "Curso")]
        public string Curso { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Digite um e-mail válido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        // OPCIONAIS

        [Display(Name = "Período")]
        public string? Periodo { get; set; }

        [Display(Name = "Cidade")]
        public string? Cidade { get; set; }

        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        // FOTO PERFIL

        [Display(Name = "Foto de Perfil")]
        public string? FotoPerfil { get; set; }

        // UPLOAD FOTO

        [NotMapped]
        [Display(Name = "Selecionar Foto")]
        public IFormFile? FotoUpload { get; set; }

        // BIO

        [MaxLength(500,
            ErrorMessage = "A bio pode ter no máximo 500 caracteres.")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        // HABILIDADES

        [Display(Name = "Habilidades")]
        public string? Habilidades { get; set; }

        // LINKS

        [Display(Name = "GitHub")]
        public string? GitHub { get; set; }

        [Display(Name = "LinkedIn")]
        public string? LinkedIn { get; set; }

        [Display(Name = "Portfólio")]
        public string? Portfolio { get; set; }

        // RELAÇÃO COM USER

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        // EXPERIÊNCIAS

        public List<Experiencia> Experiencias { get; set; }
            = new List<Experiencia>();

        public List<CandidaturaVaga> CandidaturasVagas { get; set; }
            = new List<CandidaturaVaga>();
    }
}
