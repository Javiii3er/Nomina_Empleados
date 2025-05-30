using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nomina.API.Data;
using Nomina.Shared.Models;

namespace Nomina.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly AppDbContext _context;

#pragma warning disable IDE0290
        public EmpleadosController(AppDbContext context)
        {
            _context = context;
        }
#pragma warning restore IDE0290

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            return await _context.Empleados.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> GetEmpleado(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            return empleado;
        }

        [HttpPost]
        public async Task<ActionResult<Empleado>> PostEmpleado(Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Datos inválidos",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            try
            {
                _context.Empleados.Add(empleado);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error interno al crear empleado",
                    Detail = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleado(int id, Empleado empleado)
        {
            if (id != empleado.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Datos inválidos",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var empleadoExistente = await _context.Empleados.FindAsync(id);
                if (empleadoExistente == null)
                    return NotFound();

                // Registrar cambios importantes
                var cambios = new List<string>();
                if (empleadoExistente.Salario != empleado.Salario)
                    cambios.Add($"Salario: {empleadoExistente.Salario} → {empleado.Salario}");
                if (empleadoExistente.Cargo != empleado.Cargo)
                    cambios.Add($"Cargo: {empleadoExistente.Cargo} → {empleado.Cargo}");
                if (empleadoExistente.Departamento != empleado.Departamento)
                    cambios.Add($"Departamento: {empleadoExistente.Departamento} → {empleado.Departamento}");

                if (cambios.Count > 0)

                {
                    var fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    var entrada = $"[{fecha}] Cambios: {string.Join(", ", cambios)}";
                    empleadoExistente.HistorialCambios += string.IsNullOrWhiteSpace(empleadoExistente.HistorialCambios)
                        ? entrada
                        : $"\n{entrada}";
                }

                _context.Entry(empleadoExistente).CurrentValues.SetValues(empleado);
                // No sobrescribas HistorialCambios
                _context.Entry(empleadoExistente).Property(e => e.HistorialCambios).IsModified = true;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!EmpleadoExists(id))
                    return NotFound();

                return StatusCode(500, new
                {
                    Message = "Error de concurrencia al actualizar",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error interno al actualizar empleado",
                    Detail = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            try
            {
                _context.Empleados.Remove(empleado);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error interno al eliminar empleado",
                    Detail = ex.Message
                });
            }
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
