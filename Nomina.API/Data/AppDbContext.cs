using Microsoft.EntityFrameworkCore;
using Nomina.Shared.Models;

namespace Nomina.API.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                      .HasOne(u => u.Empleado)
                      .WithOne()
                      .HasForeignKey<Usuario>(u => u.EmpleadoId);
        }
    }
}