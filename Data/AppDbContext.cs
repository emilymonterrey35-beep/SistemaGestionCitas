using Microsoft.EntityFrameworkCore;
using SistemaGestionCitas.Models;

namespace SistemaGestionCitas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Correo = "admin@sistema.com",
                    Contrasena = "123456",
                    Rol = "Administrador",
                    Activo = true
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "Usuario",
                    Correo = "usuario@sistema.com",
                    Contrasena = "123456",
                    Rol = "Usuario",
                    Activo = true
                }
            );
        }
    }
}
