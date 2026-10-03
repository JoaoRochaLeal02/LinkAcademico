using LinkAcademico.Models;

namespace LinkAcademico.ModelosDeView
{
    public class PerfilEstudanteViewModel
    {
        public Estudante Estudante { get; set; }

        public List<Experiencia> Experiencias { get; set; }
            = new();
    }
}