using Microsoft.EntityFrameworkCore;
using Nomina.Shared.Models;

namespace Nomina.API.Data;

public class AppDbContext : DbContext
{
#pragma warning disable IDE0290
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
#pragma warning restore IDE0290

    public DbSet<Empleado> Empleados { get; set; }
}
