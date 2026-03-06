using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitas.Models;
using SistemaGestionCitas.Data;

namespace SistemaGestionCitas.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UsuarioNombre = HttpContext.Session.GetString("UsuarioNombre");
            ViewBag.UsuarioRol = HttpContext.Session.GetString("UsuarioRol");
            ViewBag.TotalClientes = _context.Clientes.Count();
            ViewBag.TotalServicios = _context.Servicios.Count();
            ViewBag.TotalServiciosActivos = _context.Servicios.Count(s => s.Activo);
            ViewBag.TotalServiciosInactivos = _context.Servicios.Count(s => !s.Activo);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}