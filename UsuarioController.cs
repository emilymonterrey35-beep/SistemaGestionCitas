using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionCitas.Data;
using SistemaGestionCitas.Models;

namespace SistemaGestionCitas.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarAutenticacion()
        {
            return HttpContext.Session.GetInt32("UsuarioId") != null;
        }

        private bool EsAdministrador()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol")
                      ?? HttpContext.Session.GetString("Rol");

            return rol == "Administrador";
        }

        private IActionResult RedirigirSiNoAutorizado()
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            if (!EsAdministrador())
                return RedirectToAction("AccesoDenegado", "Account");

            return null;
        }

        public async Task<IActionResult> Index()
        {
            var acceso = RedirigirSiNoAutorizado();
            if (acceso != null) return acceso;

            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            return View(usuarios);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var acceso = RedirigirSiNoAutorizado();
            if (acceso != null) return acceso;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            var acceso = RedirigirSiNoAutorizado();
            if (acceso != null) return acceso;

            if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo))
            {
                ModelState.AddModelError("Correo", "Ya existe un usuario con ese correo.");
            }

            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Usuario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var acceso = RedirigirSiNoAutorizado();
            if (acceso != null) return acceso;

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Usuario usuario)
        {
            var acceso = RedirigirSiNoAutorizado();
            if (acceso != null) return acceso;

            if (id != usuario.Id)
                return NotFound();

            if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo && u.Id != usuario.Id))
            {
                ModelState.AddModelError("Correo", "Ya existe otro usuario con ese correo.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Usuario actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var acceso = RedirigirSiNoAutorizado();
            if (acceso != null) return acceso;

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var acceso = RedirigirSiNoAutorizado();
            if (acceso != null) return acceso;

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Activo = false;
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Usuario desactivado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}