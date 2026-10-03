using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LinkAcademico.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string TipoUsuario { get; set; } = string.Empty;
    }
}