using System.ComponentModel.DataAnnotations;

namespace LinkAcademico.Models
{
    public enum TipoAtividade
    {
        [Display(Name = "Curricular")]
        Curricular,

        [Display(Name = "Extracurricular")]
        Extracurricular
    }
}
