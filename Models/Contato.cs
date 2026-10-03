using System;

namespace LinkAcademico.Models
{
    public class Contato
    {
        public int Id { get; set; }

        public int EstudanteId { get; set; }
        public Estudante Estudante { get; set; }

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }

        public DateTime DataContato { get; set; } = DateTime.Now;

        public string Mensagem { get; set; }
    }
}