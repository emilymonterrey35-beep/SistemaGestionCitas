using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionCitas.Data;
using SistemaGestionCitas.Models;

namespace SistemaGestionCitas.Controllers
{
    public class ServicioController : Controller
    {
        private readonly AppDbContext _context;


    public ServicioController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarAutenticacion()
        {
            return HttpContext.Session.GetInt32("UsuarioId") != null;
        }

        private bool EsAdministrador()
        {
            return HttpContext.Session.GetString("UsuarioRol") == "Administrador";
        }

        public async Task<IActionResult> Index()
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            var servicios = await _context.Servicios.ToListAsync();
            return View(servicios);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            if (!VerificarAutenticacion() || !EsAdministrador())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Servicio servicio)
        {
            if (!VerificarAutenticacion() || !EsAdministrador())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Servicios.Add(servicio);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Servicio creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(servicio);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!VerificarAutenticacion() || !EsAdministrador())
                return RedirectToAction("Login", "Account");

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Servicio servicio)
        {
            if (!VerificarAutenticacion() || !EsAdministrador())
                return RedirectToAction("Login", "Account");

            if (id != servicio.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(servicio);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Servicio actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(servicio);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!VerificarAutenticacion() || !EsAdministrador())
                return RedirectToAction("Login", "Account");

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAutenticacion() || !EsAdministrador())
                return RedirectToAction("Login", "Account");

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio != null)
            {
                _context.Servicios.Remove(servicio);
                await _context.SaveChangesAsync();
            }
            TempData["Exito"] = "Servicio eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }

}