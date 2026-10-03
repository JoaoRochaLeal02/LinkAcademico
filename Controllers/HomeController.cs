using LinkAcademico.Data;
using LinkAcademico.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace LinkAcademico.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // =========================================================
        // HOME
        // =========================================================

        public async Task<IActionResult> Index()
        {
            // =========================================
            // USUÁRIO NÃO LOGADO
            // =========================================

            if (User.Identity == null ||
                !User.Identity.IsAuthenticated)
            {
                return View("Landing");
            }

            // =========================================
            // EXPERIÊNCIAS
            // =========================================

            var experiencias =
                await _context.Experiencias
                .AsNoTracking()
                .Include(e => e.Estudante)
                .OrderByDescending(e => e.Id)
                .Take(6)
                .ToListAsync();

            var totalExperiencias =
                await _context.Experiencias
                .AsNoTracking()
                .CountAsync();

            // =========================================
            // EMPRESAS
            // =========================================

            var empresas =
                await _context.Empresas
                .AsNoTracking()
                .OrderByDescending(e => e.Id)
                .Take(6)
                .ToListAsync();

            // =========================================
            // VAGAS
            // =========================================

            var vagas =
                await _context.Vagas
                .AsNoTracking()
                .Include(v => v.Empresa)
                .Include(v => v.Candidaturas)
                .Where(v => v.Ativa)
                .OrderByDescending(v => v.DataPublicacao)
                .Take(6)
                .ToListAsync();

            // =========================================
            // VAGAS COMO REFERENCIA PARA EMPRESAS
            // =========================================

            var vagasMercadoQuery =
                _context.Vagas
                .AsNoTracking()
                .Include(v => v.Empresa)
                .Include(v => v.Candidaturas)
                .Where(v => v.Ativa);

            if (User.IsInRole("Empresa"))
            {
                var userId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                var empresaLogadaId =
                    await _context.Empresas
                    .AsNoTracking()
                    .Where(e => e.UserId == userId)
                    .Select(e => (int?)e.Id)
                    .FirstOrDefaultAsync();

                if (empresaLogadaId != null)
                {
                    vagasMercadoQuery =
                        vagasMercadoQuery
                        .Where(v =>
                            v.EmpresaId != empresaLogadaId.Value);
                }
            }

            var vagasMercado =
                await vagasMercadoQuery
                .OrderByDescending(v => v.DataPublicacao)
                .Take(6)
                .ToListAsync();

            Estudante? estudanteLogado = null;
            Empresa? empresaLogada = null;

            if (User.IsInRole("Estudante"))
            {
                var userId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                estudanteLogado =
                    await _context.Estudantes
                    .AsNoTracking()
                    .Include(e => e.Experiencias)
                    .Include(e => e.CandidaturasVagas)
                    .FirstOrDefaultAsync(e =>
                        e.UserId == userId);
            }

            if (User.IsInRole("Empresa"))
            {
                var userId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                empresaLogada =
                    await _context.Empresas
                    .AsNoTracking()
                    .Include(e => e.Vagas)
                    .FirstOrDefaultAsync(e =>
                        e.UserId == userId);
            }

            // =========================================
            // ENVIA PRA VIEW
            // =========================================

            ViewBag.Empresas = empresas;
            ViewBag.Vagas = vagas;
            ViewBag.VagasMercado = vagasMercado;
            ViewBag.TotalExperiencias = totalExperiencias;
            ViewBag.EstudanteLogado = estudanteLogado;
            ViewBag.EmpresaLogada = empresaLogada;

            return View(experiencias);
        }

        // =========================================================
        // CARREGAR MAIS EXPERIENCIAS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> CarregarExperiencias(
            int skip = 0,
            int take = 6)
        {
            if (User.Identity == null ||
                !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (take < 1 || take > 12)
            {
                take = 6;
            }

            var experiencias =
                await _context.Experiencias
                .AsNoTracking()
                .Include(e => e.Estudante)
                .OrderByDescending(e => e.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            if (!experiencias.Any())
            {
                return NoContent();
            }

            return PartialView(
                "_ExperienciasFeedCards",
                experiencias);
        }

        // =========================================================
        // PRIVACY
        // =========================================================

        public IActionResult Privacy()
        {
            return View();
        }

        // =========================================================
        // ERROR
        // =========================================================

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]

        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id ??
                        HttpContext.TraceIdentifier
                });
        }
    }
}
