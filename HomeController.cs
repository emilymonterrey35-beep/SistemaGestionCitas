using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            int usuarioIdValue = usuarioId.Value;

            var usuarioNombre = HttpContext.Session.GetString("UsuarioNombre");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            ViewBag.UsuarioNombre = usuarioNombre;
            ViewBag.UsuarioRol = usuarioRol;

            if (usuarioRol == "Administrador")
            {
                // Estadísticas generales
                ViewBag.TotalUsuarios = _context.Usuarios.Count();
                ViewBag.TotalClientes = _context.Clientes.Count();

                ViewBag.TotalServicios = _context.Servicios.Count();
                ViewBag.TotalServiciosActivos = _context.Servicios.Count(s => s.Activo);
                ViewBag.TotalServiciosInactivos = _context.Servicios.Count(s => !s.Activo);

                ViewBag.TotalCitas = _context.Citas.Count();
                ViewBag.TotalCitasProgramadas = _context.Citas.Count(c => c.Estado == "Programada");
                ViewBag.TotalCitasCanceladas = _context.Citas.Count(c => c.Estado == "Cancelada");

                // Top 3 servicios más solicitados
                ViewBag.TopServicios = _context.Citas
                    .Include(c => c.Servicio)
                    .Where(c => c.Estado == "Programada" && c.Servicio != null)
                    .GroupBy(c => c.Servicio.Nombre)
                    .Select(g => new
                    {
                        NombreServicio = g.Key,
                        Total = g.Count()
                    })
                    .OrderByDescending(x => x.Total)
                    .Take(3)
                    .ToList();
            }
            else
            {
                // Datos del usuario
                ViewBag.TotalClientes = _context.Clientes.Count();

                ViewBag.TotalCitas = _context.Citas.Count(c => c.UsuarioId == usuarioIdValue);
                ViewBag.TotalCitasActivas = _context.Citas.Count(c => c.UsuarioId == usuarioIdValue && c.Estado == "Programada");
                ViewBag.TotalCitasCanceladas = _context.Citas.Count(c => c.UsuarioId == usuarioIdValue && c.Estado == "Cancelada");

                // Próximas citas (solo futuras)
                ViewBag.ProximasCitas = _context.Citas
                    .Include(c => c.Cliente)
                    .Include(c => c.Servicio)
                    .Where(c => c.UsuarioId == usuarioIdValue &&
                                c.Estado == "Programada" &&
                                c.Fecha >= DateTime.Today)
                    .OrderBy(c => c.Fecha)
                    .ThenBy(c => c.Hora)
                    .Take(5)
                    .ToList();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}