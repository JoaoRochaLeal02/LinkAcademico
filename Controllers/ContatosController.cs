using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinkAcademico.Data;
using LinkAcademico.Models;

namespace LinkAcademico.Controllers
{
    public class ContatosController : Controller
    {
        private readonly AppDbContext _context;

        public ContatosController(AppDbContext context)
        {
            _context = context;
        }

        // GET
        public IActionResult Create(int estudanteId)
        {
            var contato = new Contato
            {
                EstudanteId = estudanteId
            };

            return View(contato);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contato contato)
        {
            if (ModelState.IsValid)
            {
                contato.DataContato = DateTime.Now;

                // ⚠️ SIMPLES (empresa fixa por enquanto)
                contato.EmpresaId = 1;

                _context.Add(contato);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Estudantes", new { id = contato.EstudanteId });
            }

            return View(contato);
        }
    }
}