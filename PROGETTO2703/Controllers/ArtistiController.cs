using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROGETTO2703.Data;
using PROGETTO2703.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PROGETTO2703.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArtistiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Artisti
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Artista>>> Get()
        {
            return Ok(await _context.Artisti.ToListAsync());
        }

        // GET: api/Artisti/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Artista>> Get(int id)
        {
            var artista = await _context.Artisti.FindAsync(id);
            if (artista == null) return NotFound();
            return artista;
        }

        // POST: api/Artisti
        [HttpPost]
        [Authorize(Roles = "Amministratore")]
        public async Task<IActionResult> Post(Artista artista)
        {
            _context.Artisti.Add(artista);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = artista.ArtistaId }, artista);
        }

        // PUT: api/Artisti/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Amministratore")]
        public async Task<IActionResult> Put(int id, Artista artista)
        {
            if (id != artista.ArtistaId) return BadRequest();
            _context.Entry(artista).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Artisti/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Amministratore")]
        public async Task<IActionResult> Delete(int id)
        {
            var artista = await _context.Artisti.FindAsync(id);
            if (artista == null) return NotFound();

            _context.Artisti.Remove(artista);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
