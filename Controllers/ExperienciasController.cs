using System.Security.Claims;
using LinkAcademico.Data;
using LinkAcademico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkAcademico.Controllers
{
    [Authorize]
    public class ExperienciasController : Controller
    {
        private readonly AppDbContext _context;

        public ExperienciasController(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // MEU PORTFÓLIO / MINHAS EXPERIÊNCIAS
        // =========================================================

        [Authorize(Roles = "Estudante")]
        public async Task<IActionResult> Index()
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var experiencias =
                await _context.Experiencias

                .AsNoTracking()

                .Include(e => e.Estudante)

                .Where(e =>
                    e.Estudante.UserId == userId)

                .OrderByDescending(e => e.Data)

                .ToListAsync();

            ViewBag.TotalExperiencias =
                experiencias.Count;

            return View(experiencias);
        }

        // =========================================================
        // 🌐 DETALHES PÚBLICOS DO PROJETO
        // =========================================================

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var experiencia =
                await _context.Experiencias

                .AsNoTracking()

                .Include(e => e.Estudante)

                .FirstOrDefaultAsync(e =>
                    e.Id == id);

            if (experiencia == null)
                return NotFound();

            return View(experiencia);
        }

        // =========================================================
        // CREATE GET
        // =========================================================

        [Authorize(Roles = "Estudante")]
        public async Task<IActionResult> Create()
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var estudante =
                await _context.Estudantes

                .AsNoTracking()

                .FirstOrDefaultAsync(e =>
                    e.UserId == userId);

            if (estudante == null)
            {
                TempData["Erro"] =
                    "Você precisa criar seu perfil antes de adicionar experiências.";

                return RedirectToAction(
                    "Create",
                    "Estudantes");
            }

            return View();
        }

        // =========================================================
        // CREATE POST
        // =========================================================

        [Authorize(Roles = "Estudante")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(

            [Bind(
                "Titulo," +
                "Descricao," +
                "Categoria," +
                "TipoAtividade," +
                "Data," +
                "Instituicao," +
                "Competencias," +
                "CertificadoUrl," +
                "ImagemUrl," +
                "ProjetoUrl")]

            Experiencia experiencia)
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var estudante =
                await _context.Estudantes

                .FirstOrDefaultAsync(e =>
                    e.UserId == userId);

            if (estudante == null)
            {
                TempData["Erro"] =
                    "Você precisa criar um perfil.";

                return RedirectToAction(
                    "Create",
                    "Estudantes");
            }

            experiencia.EstudanteId =
                estudante.Id;

            if (!ModelState.IsValid)
            {
                return View(experiencia);
            }

            _context.Experiencias.Add(experiencia);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                "Experiência publicada com sucesso!";

            // 🔥 REDIRECIONA PARA HOME / FEED

            return RedirectToAction(
                "Index",
                "Home");
        }

        // =========================================================
        // EDIT GET
        // =========================================================

        [Authorize(Roles = "Estudante")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var experiencia =
                await _context.Experiencias

                .AsNoTracking()

                .Include(e => e.Estudante)

                .FirstOrDefaultAsync(e =>

                    e.Id == id &&
                    e.Estudante.UserId == userId);

            if (experiencia == null)
                return NotFound();

            return View(experiencia);
        }

        // =========================================================
        // EDIT POST
        // =========================================================

        [Authorize(Roles = "Estudante")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(

            int id,

            [Bind(
                "Id," +
                "Titulo," +
                "Descricao," +
                "Categoria," +
                "TipoAtividade," +
                "Data," +
                "Instituicao," +
                "Competencias," +
                "CertificadoUrl," +
                "ImagemUrl," +
                "ProjetoUrl")]

            Experiencia experiencia)
        {
            if (id != experiencia.Id)
                return NotFound();

            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var experienciaBanco =
                await _context.Experiencias

                .Include(e => e.Estudante)

                .AsNoTracking()

                .FirstOrDefaultAsync(e =>

                    e.Id == id &&
                    e.Estudante.UserId == userId);

            if (experienciaBanco == null)
                return NotFound();

            experiencia.EstudanteId =
                experienciaBanco.EstudanteId;

            if (!ModelState.IsValid)
            {
                return View(experiencia);
            }

            try
            {
                _context.Update(experiencia);

                await _context.SaveChangesAsync();

                TempData["Sucesso"] =
                    "Experiência atualizada!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Experiencias.Any(e =>
                    e.Id == experiencia.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // DELETE GET
        // =========================================================

        [Authorize(Roles = "Estudante")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var experiencia =
                await _context.Experiencias

                .AsNoTracking()

                .Include(e => e.Estudante)

                .FirstOrDefaultAsync(e =>

                    e.Id == id &&
                    e.Estudante.UserId == userId);

            if (experiencia == null)
                return NotFound();

            return View(experiencia);
        }

        // =========================================================
        // DELETE POST
        // =========================================================

        [Authorize(Roles = "Estudante")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var experiencia =
                await _context.Experiencias

                .Include(e => e.Estudante)

                .FirstOrDefaultAsync(e =>

                    e.Id == id &&
                    e.Estudante.UserId == userId);

            if (experiencia != null)
            {
                _context.Experiencias.Remove(experiencia);

                await _context.SaveChangesAsync();

                TempData["Sucesso"] =
                    "Experiência removida!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
