using Microsoft.EntityFrameworkCore;
using Nomina.Shared.Models;

namespace Nomina.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Empleado> Empleados { get; set; }
}
