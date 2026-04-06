using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestionCitas.Data;
using SistemaGestionCitas.Models;

namespace SistemaGestionCitas.Controllers
{
    public class CitaController : Controller
    {
        private readonly AppDbContext _context;

        public CitaController(AppDbContext context)
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

        private int? UsuarioLogueadoId()
        {
            return HttpContext.Session.GetInt32("UsuarioId");
        }

        private async Task CargarCombos(int? usuarioSeleccionado = null)
        {
            ViewBag.Clientes = new SelectList(
                await _context.Clientes
                    .OrderBy(c => c.NombreCompleto)
                    .ToListAsync(),
                "Id",
                "NombreCompleto"
            );

            ViewBag.Servicios = new SelectList(
                await _context.Servicios
                    .Where(s => s.Activo)
                    .OrderBy(s => s.Nombre)
                    .ToListAsync(),
                "Id",
                "Nombre"
            );

            var usuarios = await _context.Usuarios
                .Where(u => u.Activo && u.Rol == "Usuario")
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            ViewBag.Usuarios = new SelectList(
                usuarios,
                "Id",
                "Nombre",
                usuarioSeleccionado
            );
        }

        public async Task<IActionResult> Index()
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            IQueryable<Cita> query = _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Include(c => c.Usuario);

            if (!EsAdministrador())
            {
                int? usuarioId = UsuarioLogueadoId();
                if (usuarioId == null)
                    return RedirectToAction("Login", "Account");

                int usuarioIdValue = usuarioId.Value;
                query = query.Where(c => c.UsuarioId == usuarioIdValue);
            }

            var citas = await query
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Hora)
                .ToListAsync();

            return View(citas);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            if (EsAdministrador())
                return RedirectToAction("AccesoDenegado", "Account");

            int? usuarioId = UsuarioLogueadoId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            await CargarCombos(usuarioId);

            var cita = new Cita
            {
                Fecha = DateTime.Today,
                Estado = "Programada",
                UsuarioId = usuarioId.Value
            };

            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cita cita)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            if (EsAdministrador())
                return RedirectToAction("AccesoDenegado", "Account");

            int? usuarioId = UsuarioLogueadoId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            int usuarioLogueado = usuarioId.Value;
            cita.UsuarioId = usuarioLogueado;

            var servicio = await _context.Servicios.FindAsync(cita.ServicioId);
            if (servicio == null || !servicio.Activo)
            {
                ModelState.AddModelError("ServicioId", "No se puede crear una cita con un servicio inactivo.");
            }

            var fechaHoraCita = cita.Fecha.Date + cita.Hora;
            if (fechaHoraCita <= DateTime.Now)
            {
                ModelState.AddModelError("Fecha", "No se pueden crear citas en fechas u horas pasadas.");
            }

            cita.Estado = "Programada";

            if (ModelState.IsValid)
            {
                _context.Citas.Add(cita);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Cita creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            await CargarCombos(cita.UsuarioId);
            return View(cita);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
                return NotFound();

            int? usuarioId = UsuarioLogueadoId();
            if (!EsAdministrador() && cita.UsuarioId != usuarioId)
                return RedirectToAction("AccesoDenegado", "Account");

            if (cita.Estado == "Cancelada")
            {
                TempData["Exito"] = "Una cita cancelada no puede modificarse.";
                return RedirectToAction(nameof(Index));
            }

            await CargarCombos(cita.UsuarioId);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Cita cita)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            if (id != cita.Id)
                return NotFound();

            var citaBD = await _context.Citas
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (citaBD == null)
                return NotFound();

            int? usuarioId = UsuarioLogueadoId();
            if (!EsAdministrador() && citaBD.UsuarioId != usuarioId)
                return RedirectToAction("AccesoDenegado", "Account");

            if (citaBD.Estado == "Cancelada")
            {
                TempData["Exito"] = "Una cita cancelada no puede modificarse.";
                return RedirectToAction(nameof(Index));
            }

            if (!EsAdministrador())
            {
                cita.UsuarioId = citaBD.UsuarioId;
            }

            var servicio = await _context.Servicios.FindAsync(cita.ServicioId);
            if (servicio == null || !servicio.Activo)
            {
                ModelState.AddModelError("ServicioId", "No se puede usar un servicio inactivo.");
            }

            var fechaHoraCita = cita.Fecha.Date + cita.Hora;
            if (fechaHoraCita <= DateTime.Now)
            {
                ModelState.AddModelError("Fecha", "No se pueden registrar citas en fechas u horas pasadas.");
            }

            if (string.IsNullOrWhiteSpace(cita.Estado))
                cita.Estado = "Programada";

            if (ModelState.IsValid)
            {
                _context.Update(cita);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Cita actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            await CargarCombos(cita.UsuarioId);
            return View(cita);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
                return NotFound();

            int? usuarioId = UsuarioLogueadoId();
            if (!EsAdministrador() && cita.UsuarioId != usuarioId)
                return RedirectToAction("AccesoDenegado", "Account");

            return View(cita);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Account");

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
                return NotFound();

            int? usuarioId = UsuarioLogueadoId();
            if (!EsAdministrador() && cita.UsuarioId != usuarioId)
                return RedirectToAction("AccesoDenegado", "Account");

            if (cita.Estado == "Cancelada")
            {
                TempData["Exito"] = "La cita ya estaba cancelada.";
                return RedirectToAction(nameof(Index));
            }

            cita.Estado = "Cancelada";
            _context.Update(cita);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Cita cancelada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}