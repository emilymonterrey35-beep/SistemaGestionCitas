using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionCitas.Data;
using SistemaGestionCitas.Models;

namespace SistemaGestionCitas.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarAutenticacion()
        {
            return HttpContext.Session.GetInt32("UsuarioId") != null;
        }

        public async Task<IActionResult> Index()
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            var clientes = await _context.Clientes.ToListAsync();
            return View(clientes);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cliente cliente)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Cliente creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Cliente cliente)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            if (id != cliente.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Cliente actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Cliente eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
