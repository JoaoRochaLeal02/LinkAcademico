using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LinkAcademico.Data;
using LinkAcademico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkAcademico.Controllers
{
    [Authorize]
    public class VagasController : Controller
    {
        private readonly AppDbContext _context;

        public VagasController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string busca)
        {
            var vagas =
                _context.Vagas
                .AsNoTracking()
                .Include(v => v.Empresa)
                .Include(v => v.Candidaturas)
                .Where(v => v.Ativa)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                vagas =
                    vagas.Where(v =>
                        v.Titulo.Contains(busca) ||
                        v.Descricao.Contains(busca) ||
                        v.Requisitos.Contains(busca) ||
                        (v.Empresa != null &&
                         v.Empresa.Nome.Contains(busca)));
            }

            var lista =
                await vagas
                .OrderByDescending(v => v.DataPublicacao)
                .ToListAsync();

            return View(lista);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vaga =
                await _context.Vagas
                .AsNoTracking()
                .Include(v => v.Empresa)
                .Include(v => v.Candidaturas)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vaga == null)
            {
                return NotFound();
            }

            ViewBag.JaCandidatou = false;
            ViewBag.EstudanteId = 0;

            if (User.Identity != null &&
                User.Identity.IsAuthenticated &&
                User.IsInRole("Estudante"))
            {
                var estudante =
                    await ObterEstudanteLogadoAsync();

                if (estudante != null)
                {
                    ViewBag.EstudanteId =
                        estudante.Id;

                    ViewBag.JaCandidatou =
                        await _context.CandidaturasVagas
                        .AnyAsync(c =>
                            c.VagaId == vaga.Id &&
                            c.EstudanteId == estudante.Id);
                }
            }

            return View(vaga);
        }

        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Minhas()
        {
            var empresa =
                await ObterEmpresaLogadaAsync();

            if (empresa == null)
            {
                return RedirectToAction(
                    "Create",
                    "Empresas");
            }

            var vagas =
                await _context.Vagas
                .AsNoTracking()
                .Include(v => v.Candidaturas)
                .Where(v => v.EmpresaId == empresa.Id)
                .OrderByDescending(v => v.DataPublicacao)
                .ToListAsync();

            return View(vagas);
        }

        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Create()
        {
            var empresa =
                await ObterEmpresaLogadaAsync();

            if (empresa == null)
            {
                return RedirectToAction(
                    "Create",
                    "Empresas");
            }

            return View(
                new Vaga
                {
                    Ativa = true
                });
        }

        [Authorize(Roles = "Empresa")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vaga vaga)
        {
            var empresa =
                await ObterEmpresaLogadaAsync();

            if (empresa == null)
            {
                return RedirectToAction(
                    "Create",
                    "Empresas");
            }

            vaga.EmpresaId = empresa.Id;
            vaga.DataPublicacao = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return View(vaga);
            }

            _context.Vagas.Add(vaga);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                "Vaga publicada com sucesso.";

            return RedirectToAction(nameof(Minhas));
        }

        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa =
                await ObterEmpresaLogadaAsync();

            if (empresa == null)
            {
                return RedirectToAction(
                    "Create",
                    "Empresas");
            }

            var vaga =
                await _context.Vagas
                .FirstOrDefaultAsync(v =>
                    v.Id == id &&
                    v.EmpresaId == empresa.Id);

            if (vaga == null)
            {
                return Forbid();
            }

            return View(vaga);
        }

        [Authorize(Roles = "Empresa")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vaga vaga)
        {
            if (id != vaga.Id)
            {
                return NotFound();
            }

            var empresa =
                await ObterEmpresaLogadaAsync();

            if (empresa == null)
            {
                return RedirectToAction(
                    "Create",
                    "Empresas");
            }

            var vagaBanco =
                await _context.Vagas
                .AsNoTracking()
                .FirstOrDefaultAsync(v =>
                    v.Id == id &&
                    v.EmpresaId == empresa.Id);

            if (vagaBanco == null)
            {
                return Forbid();
            }

            vaga.EmpresaId = empresa.Id;
            vaga.DataPublicacao = vagaBanco.DataPublicacao;

            if (!ModelState.IsValid)
            {
                return View(vaga);
            }

            _context.Update(vaga);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                "Vaga atualizada com sucesso.";

            return RedirectToAction(nameof(Minhas));
        }

        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa =
                await ObterEmpresaLogadaAsync();

            if (empresa == null)
            {
                return RedirectToAction(
                    "Create",
                    "Empresas");
            }

            var vaga =
                await _context.Vagas
                .AsNoTracking()
                .FirstOrDefaultAsync(v =>
                    v.Id == id &&
                    v.EmpresaId == empresa.Id);

            if (vaga == null)
            {
                return Forbid();
            }

            return View(vaga);
        }

        [Authorize(Roles = "Empresa")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empresa =
                await ObterEmpresaLogadaAsync();

            if (empresa == null)
            {
                return RedirectToAction(
                    "Create",
                    "Empresas");
            }

            var vaga =
                await _context.Vagas
                .FirstOrDefaultAsync(v =>
                    v.Id == id &&
                    v.EmpresaId == empresa.Id);

            if (vaga == null)
            {
                return Forbid();
            }

            _context.Vagas.Remove(vaga);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                "Vaga removida.";

            return RedirectToAction(nameof(Minhas));
        }

        [Authorize(Roles = "Estudante")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Candidatar(int vagaId, string? mensagem)
        {
            var estudante =
                await ObterEstudanteLogadoAsync();

            if (estudante == null)
            {
                TempData["Erro"] =
                    "Crie seu perfil de estudante antes de se candidatar.";

                return RedirectToAction(
                    "Create",
                    "Estudantes");
            }

            var vaga =
                await _context.Vagas
                .Include(v => v.Empresa)
                .FirstOrDefaultAsync(v =>
                    v.Id == vagaId &&
                    v.Ativa);

            if (vaga == null)
            {
                return NotFound();
            }

            var jaCandidatou =
                await _context.CandidaturasVagas
                .AnyAsync(c =>
                    c.VagaId == vaga.Id &&
                    c.EstudanteId == estudante.Id);

            if (jaCandidatou)
            {
                TempData["Erro"] =
                    "Você já se candidatou a esta vaga.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = vaga.Id });
            }



            _context.CandidaturasVagas.Add(
                new CandidaturaVaga
                {
                    VagaId = vaga.Id,
                    EstudanteId = estudante.Id,
                    Mensagem = mensagem,
                    DataCandidatura = DateTime.Now
                });

            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                "Candidatura enviada com sucesso.";

            return RedirectToAction(
                nameof(Details),
                new { id = vaga.Id });
        }

        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Candidatos(int id)
        {
            var empresa =
                await ObterEmpresaLogadaAsync();

            if (empresa == null)
            {
                return RedirectToAction(
                    "Create",
                    "Empresas");
            }

            var vaga =
                await _context.Vagas
                .AsNoTracking()
                .Include(v => v.Candidaturas)
                    .ThenInclude(c => c.Estudante)
                        .ThenInclude(e => e!.Experiencias)
                .FirstOrDefaultAsync(v =>
                    v.Id == id &&
                    v.EmpresaId == empresa.Id);

            if (vaga == null)
            {
                return Forbid();
            }

            return View(vaga);
        }

        private async Task<Empresa?> ObterEmpresaLogadaAsync()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return await _context.Empresas
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId);
        }

        private async Task<Estudante?> ObterEstudanteLogadoAsync()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return await _context.Estudantes
                .Include(e => e.Experiencias)
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId);
        }

    }
}
