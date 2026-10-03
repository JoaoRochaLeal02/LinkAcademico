using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LinkAcademico.Models;

namespace LinkAcademico.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Estudante> Estudantes { get; set; }

        public DbSet<Empresa> Empresas { get; set; }

        public DbSet<Experiencia> Experiencias { get; set; }

        public DbSet<Contato> Contatos { get; set; }

        public DbSet<Vaga> Vagas { get; set; }

        public DbSet<CandidaturaVaga> CandidaturasVagas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🔐 RELAÇÃO ESTUDANTE -> USER
            builder.Entity<Estudante>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 🔐 RELAÇÃO EMPRESA -> USER
            builder.Entity<Empresa>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Vaga>()
                .HasOne(v => v.Empresa)
                .WithMany(e => e.Vagas)
                .HasForeignKey(v => v.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CandidaturaVaga>()
                .HasOne(c => c.Vaga)
                .WithMany(v => v.Candidaturas)
                .HasForeignKey(c => c.VagaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CandidaturaVaga>()
                .HasOne(c => c.Estudante)
                .WithMany(e => e.CandidaturasVagas)
                .HasForeignKey(c => c.EstudanteId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CandidaturaVaga>()
                .HasIndex(c => new { c.VagaId, c.EstudanteId })
                .IsUnique();
        }
    }
}
