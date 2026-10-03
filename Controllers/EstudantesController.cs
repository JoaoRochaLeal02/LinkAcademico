using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using LinkAcademico.Data;
using LinkAcademico.Models;
using LinkAcademico.ModelosDeView;

namespace LinkAcademico.Controllers
{
    [Authorize]
    public class EstudantesController : Controller
    {
        private readonly AppDbContext _context;

        private readonly IWebHostEnvironment
            _webHostEnvironment;

        public EstudantesController(
            AppDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;

            _webHostEnvironment =
                webHostEnvironment;
        }

        // =========================================================
        // INDEX
        // =========================================================

        public async Task<IActionResult> Index()
        {
            // EMPRESA VISUALIZA TALENTOS

            if (User.IsInRole("Empresa"))
            {
                var estudantesEmpresa =
                    await _context.Estudantes
                    .Include(e => e.Experiencias)
                    .ToListAsync();

                return View(
                    "Buscar",
                    estudantesEmpresa);
            }

            // ESTUDANTE

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var estudante =
                await _context.Estudantes
                .Include(e => e.Experiencias)
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId);

            // NÃO POSSUI PERFIL

            if (estudante == null)
            {
                return RedirectToAction(
                    nameof(Create));
            }

            // POSSUI PERFIL

            return RedirectToAction(
                nameof(Details),
                new { id = estudante.Id });
        }

        // =========================================================
        // PERFIL PÚBLICO
        // =========================================================

        [AllowAnonymous]
        public async Task<IActionResult> Perfil(
            int id)
        {
            var estudante =
                await _context.Estudantes
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == id);

            if (estudante == null)
            {
                return NotFound();
            }

            var experiencias =
                await _context.Experiencias
                .AsNoTracking()
                .Where(e =>
                    e.EstudanteId == id)
                .OrderByDescending(e =>
                    e.Data)
                .ToListAsync();

            var viewModel =
                new PerfilEstudanteViewModel
                {
                    Estudante = estudante,
                    Experiencias = experiencias
                };

            return View(viewModel);
        }

        // =========================================================
        // DETAILS
        // =========================================================

        public async Task<IActionResult> Details(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estudante =
                await _context.Estudantes
                .Include(e => e.Experiencias)
                .FirstOrDefaultAsync(e =>
                    e.Id == id);

            if (estudante == null)
            {
                return NotFound();
            }

            // VERIFICA DONO

            bool ehDono = false;

            if (User.Identity != null &&
                User.Identity.IsAuthenticated)
            {
                var userId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                ehDono =
                    estudante.UserId == userId;
            }

            ViewBag.EhDono = ehDono;

            return View(estudante);
        }

        // =========================================================
        // BUSCAR
        // =========================================================

        public async Task<IActionResult> Buscar(
            string nome,
            string categoria)
        {
            var estudantes =
                _context.Estudantes
                .Include(e => e.Experiencias)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                estudantes =
                    estudantes.Where(e =>
                        e.Nome.Contains(nome));
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                estudantes =
                    estudantes.Where(e =>
                        e.Experiencias.Any(exp =>
                            exp.Categoria
                                .ToString()
                                .Contains(categoria)));
            }

            return View(
                await estudantes.ToListAsync());
        }

        // =========================================================
        // CREATE GET
        // =========================================================

        [Authorize(Roles = "Estudante")]
        public async Task<IActionResult> Create()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var estudanteExistente =
                await _context.Estudantes
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId);

            if (estudanteExistente != null)
            {
                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id =
                        estudanteExistente.Id
                    });
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
            Estudante estudante)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var estudanteExiste =
                await _context.Estudantes
                .AnyAsync(e =>
                    e.UserId == userId);

            // JÁ POSSUI PERFIL

            if (estudanteExiste)
            {
                return RedirectToAction(
                    nameof(Index));
            }

            estudante.UserId = userId;

            // FOTO

            if (estudante.FotoUpload != null &&
                estudante.FotoUpload.Length > 0)
            {
                string pasta =
                    Path.Combine(
                        _webHostEnvironment
                            .WebRootPath,
                        "uploads");

                if (!Directory.Exists(pasta))
                {
                    Directory.CreateDirectory(
                        pasta);
                }

                string nomeArquivo =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(
                        estudante.FotoUpload.FileName);

                string caminhoCompleto =
                    Path.Combine(
                        pasta,
                        nomeArquivo);

                using (var stream =
                    new FileStream(
                        caminhoCompleto,
                        FileMode.Create))
                {
                    await estudante.FotoUpload
                        .CopyToAsync(stream);
                }

                estudante.FotoPerfil =
                    "/uploads/" +
                    nomeArquivo;
            }

            if (!ModelState.IsValid)
            {
                return View(estudante);
            }

            _context.Estudantes
                .Add(estudante);

            await _context
                .SaveChangesAsync();

            return RedirectToAction(
                nameof(Details),
                new { id = estudante.Id });
        }

        // =========================================================
        // EDIT GET
        // =========================================================

        [Authorize(Roles = "Estudante")]
        public async Task<IActionResult> Edit(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var estudante =
                await _context.Estudantes
                .FirstOrDefaultAsync(e =>
                    e.Id == id &&
                    e.UserId == userId);

            if (estudante == null)
            {
                return Forbid();
            }

            return View(estudante);
        }

        // =========================================================
        // EDIT POST
        // =========================================================

        [Authorize(Roles = "Estudante")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(
            int id,
            Estudante estudante)
        {
            if (id != estudante.Id)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var estudanteBanco =
                await _context.Estudantes
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == id &&
                    e.UserId == userId);

            if (estudanteBanco == null)
            {
                return Forbid();
            }

            estudante.UserId = userId;

            // MANTÉM FOTO ANTIGA

            estudante.FotoPerfil =
                estudanteBanco.FotoPerfil;

            // NOVA FOTO

            if (estudante.FotoUpload != null &&
                estudante.FotoUpload.Length > 0)
            {
                string pasta =
                    Path.Combine(
                        _webHostEnvironment
                            .WebRootPath,
                        "uploads");

                if (!Directory.Exists(pasta))
                {
                    Directory.CreateDirectory(
                        pasta);
                }

                string nomeArquivo =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(
                        estudante.FotoUpload.FileName);

                string caminhoCompleto =
                    Path.Combine(
                        pasta,
                        nomeArquivo);

                using (var stream =
                    new FileStream(
                        caminhoCompleto,
                        FileMode.Create))
                {
                    await estudante.FotoUpload
                        .CopyToAsync(stream);
                }

                estudante.FotoPerfil =
                    "/uploads/" +
                    nomeArquivo;
            }

            if (!ModelState.IsValid)
            {
                return View(estudante);
            }

            try
            {
                _context.Update(estudante);

                await _context
                    .SaveChangesAsync();
            }
            catch (
                DbUpdateConcurrencyException)
            {
                if (!EstudanteExists(
                    estudante.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(
                nameof(Details),
                new { id = estudante.Id });
        }

        // =========================================================
        // DELETE GET
        // =========================================================

        [Authorize(Roles = "Estudante")]
        public async Task<IActionResult> Delete(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var estudante =
                await _context.Estudantes
                .FirstOrDefaultAsync(e =>
                    e.Id == id &&
                    e.UserId == userId);

            if (estudante == null)
            {
                return Forbid();
            }

            return View(estudante);
        }

        // =========================================================
        // DELETE POST
        // =========================================================

        [Authorize(Roles = "Estudante")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var estudante =
                await _context.Estudantes
                .FirstOrDefaultAsync(e =>
                    e.Id == id &&
                    e.UserId == userId);

            if (estudante == null)
            {
                return Forbid();
            }

            // REMOVE FOTO

            if (!string.IsNullOrEmpty(
                estudante.FotoPerfil))
            {
                var caminhoFoto =
                    Path.Combine(
                        _webHostEnvironment
                            .WebRootPath,
                        estudante.FotoPerfil
                            .TrimStart('/'));

                if (System.IO.File.Exists(
                    caminhoFoto))
                {
                    System.IO.File.Delete(
                        caminhoFoto);
                }
            }

            _context.Estudantes
                .Remove(estudante);

            await _context
                .SaveChangesAsync();

            await HttpContext
                .SignOutAsync();

            return RedirectToAction(
                "Index",
                "Home");
        }

        // =========================================================
        // EXISTS
        // =========================================================

        private bool EstudanteExists(
            int id)
        {
            return _context.Estudantes
                .Any(e => e.Id == id);
        }
    }
}