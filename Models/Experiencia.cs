using System;
using System.ComponentModel.DataAnnotations;

namespace LinkAcademico.Models
{
    public class Experiencia
    {
        public int Id { get; set; }

        // TÍTULO

        [Required(ErrorMessage = "O título é obrigatório.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        // DESCRIÇÃO

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        // CATEGORIA

        [Display(Name = "Categoria")]
        public CategoriaExperiencia Categoria { get; set; }

        [Display(Name = "Tipo de Atividade")]
        public TipoAtividade TipoAtividade { get; set; }

        // DATA

        [Required(ErrorMessage = "A data é obrigatória.")]
        [Display(Name = "Data")]
        public DateTime Data { get; set; }

        // INSTITUIÇÃO

        [Display(Name = "Instituição/Empresa")]
        public string? Instituicao { get; set; }

        // COMPETÊNCIAS

        [Display(Name = "Competências")]
        public string? Competencias { get; set; }

        // CERTIFICADO

        [Display(Name = "Link do Certificado")]
        public string? CertificadoUrl { get; set; }

        // IMAGEM DO PROJETO

        [Display(Name = "Imagem do Projeto")]
        public string? ImagemUrl { get; set; }

        // LINK DO PROJETO

        [Display(Name = "Link do Projeto")]
        public string? ProjetoUrl { get; set; }

        // RELAÇÃO COM ESTUDANTE

        public int EstudanteId { get; set; }

        public Estudante? Estudante { get; set; }
    }
}
