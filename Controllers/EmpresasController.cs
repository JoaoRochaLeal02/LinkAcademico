using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LinkAcademico.Data;
using LinkAcademico.Models;

namespace LinkAcademico.Controllers
{
    [Authorize]
    public class EmpresasController : Controller
    {
        private readonly AppDbContext _context;

        public EmpresasController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // PERFIL DA EMPRESA LOGADA
        // =====================================================

        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Perfil()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var empresa =
                await _context.Empresas
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId);

            // NÃO POSSUI PERFIL

            if (empresa == null)
            {
                return RedirectToAction(
                    nameof(Create));
            }

            // POSSUI PERFIL

            return RedirectToAction(
                nameof(Details),
                new { id = empresa.Id });
        }

        // =====================================================
        // LISTA
        // =====================================================

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var empresas =
                await _context.Empresas
                .Include(e => e.Vagas)
                .OrderBy(e => e.Nome)
                .ToListAsync();

            return View(empresas);
        }

        // =====================================================
        // DETAILS
        // =====================================================

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa =
                await _context.Empresas
                .Include(e => e.Vagas)
                .FirstOrDefaultAsync(e =>
                    e.Id == id);

            if (empresa == null)
            {
                return NotFound();
            }

            bool ehDono = false;

            if (User.Identity != null &&
                User.Identity.IsAuthenticated)
            {
                var userId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                ehDono =
                    empresa.UserId == userId;
            }

            ViewBag.EhDono = ehDono;

            return View(empresa);
        }

        // =====================================================
        // CREATE GET
        // =====================================================

        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Create()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var empresaExistente =
                await _context.Empresas
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId);

            // JÁ POSSUI PERFIL

            if (empresaExistente != null)
            {
                return RedirectToAction(
                    nameof(Details),
                    new { id = empresaExistente.Id });
            }

            return View();
        }

        // =====================================================
        // CREATE POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Empresa")]

        public async Task<IActionResult> Create(
            Empresa empresa,
            IFormFile? foto)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var empresaExiste =
                await _context.Empresas
                .AnyAsync(e =>
                    e.UserId == userId);

            // JÁ POSSUI PERFIL

            if (empresaExiste)
            {
                return RedirectToAction(
                    nameof(Perfil));
            }

            empresa.UserId = userId;

            if (!ModelState.IsValid)
            {
                return View(empresa);
            }

            // =========================================
            // FOTO
            // =========================================

            if (foto != null &&
                foto.Length > 0)
            {
                var nomeArquivo =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(foto.FileName);

                var pasta =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads");

                // CRIA PASTA

                if (!Directory.Exists(pasta))
                {
                    Directory.CreateDirectory(
                        pasta);
                }

                var caminhoCompleto =
                    Path.Combine(
                        pasta,
                        nomeArquivo);

                using (var stream =
                    new FileStream(
                        caminhoCompleto,
                        FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                empresa.FotoPerfil =
                    "/uploads/" + nomeArquivo;
            }

            _context.Empresas.Add(empresa);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Details),
                new { id = empresa.Id });
        }

        // =====================================================
        // EDIT GET
        // =====================================================

        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var empresa =
                await _context.Empresas
                .FirstOrDefaultAsync(e =>
                    e.Id == id &&
                    e.UserId == userId);

            // SEGURANÇA

            if (empresa == null)
            {
                return Forbid();
            }

            return View(empresa);
        }

        // =====================================================
        // EDIT POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Empresa")]

        public async Task<IActionResult> Edit(
            int id,
            Empresa empresa,
            IFormFile? foto)
        {
            if (id != empresa.Id)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var empresaBanco =
                await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == id &&
                    e.UserId == userId);

            // SEGURANÇA

            if (empresaBanco == null)
            {
                return Forbid();
            }

            empresa.UserId = userId;

            // MANTÉM FOTO ANTIGA

            empresa.FotoPerfil =
                empresaBanco.FotoPerfil;

            if (!ModelState.IsValid)
            {
                return View(empresa);
            }

            // =========================================
            // NOVA FOTO
            // =========================================

            if (foto != null &&
                foto.Length > 0)
            {
                var nomeArquivo =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(foto.FileName);

                var pasta =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads");

                if (!Directory.Exists(pasta))
                {
                    Directory.CreateDirectory(
                        pasta);
                }

                var caminhoCompleto =
                    Path.Combine(
                        pasta,
                        nomeArquivo);

                using (var stream =
                    new FileStream(
                        caminhoCompleto,
                        FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                empresa.FotoPerfil =
                    "/uploads/" + nomeArquivo;
            }

            try
            {
                _context.Update(empresa);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpresaExists(
                    empresa.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(
                nameof(Details),
                new { id = empresa.Id });
        }

        // =====================================================
        // DELETE GET
        // =====================================================

        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var empresa =
                await _context.Empresas
                .FirstOrDefaultAsync(e =>
                    e.Id == id &&
                    e.UserId == userId);

            if (empresa == null)
            {
                return Forbid();
            }

            return View(empresa);
        }

        // =====================================================
        // DELETE POST
        // =====================================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Empresa")]

        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var empresa =
                await _context.Empresas
                .FirstOrDefaultAsync(e =>
                    e.Id == id &&
                    e.UserId == userId);

            if (empresa == null)
            {
                return Forbid();
            }

            // REMOVE FOTO

            if (!string.IsNullOrEmpty(
                empresa.FotoPerfil))
            {
                var caminhoFoto =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        empresa.FotoPerfil
                            .TrimStart('/'));

                if (System.IO.File.Exists(
                    caminhoFoto))
                {
                    System.IO.File.Delete(
                        caminhoFoto);
                }
            }

            _context.Empresas.Remove(empresa);

            await _context.SaveChangesAsync();

            // DESLOGA

            await HttpContext.SignOutAsync();

            return RedirectToAction(
                "Index",
                "Home");
        }

        // =====================================================
        // EXISTS
        // =====================================================

        private bool EmpresaExists(int id)
        {
            return _context.Empresas
                .Any(e => e.Id == id);
        }
    }
}
