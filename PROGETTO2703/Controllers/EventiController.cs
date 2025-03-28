using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROGETTO2703.Data;
using PROGETTO2703.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PROGETTO2703.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class EventiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Eventi
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Evento>>> Get()
        {
            return Ok(await _context.Eventi.Include(e => e.Artista).ToListAsync());
        }

        // GET: api/Eventi/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Evento>> Get(int id)
        {
            var evento = await _context.Eventi.Include(e => e.Artista)
                                              .FirstOrDefaultAsync(e => e.EventoId == id);
            if (evento == null) return NotFound();
            return evento;
        }

        // POST: api/Eventi
        [HttpPost]
        [Authorize(Roles = "Amministratore")]
        public async Task<IActionResult> Post(Evento evento)
        {
            _context.Eventi.Add(evento);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = evento.EventoId }, evento);
        }

        // PUT: api/Eventi/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Amministratore")]
        public async Task<IActionResult> Put(int id, Evento evento)
        {
            if (id != evento.EventoId) return BadRequest();
            _context.Entry(evento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Eventi/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Amministratore")]
        public async Task<IActionResult> Delete(int id)
        {
            var evento = await _context.Eventi.FindAsync(id);
            if (evento == null) return NotFound();

            _context.Eventi.Remove(evento);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

