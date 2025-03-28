using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROGETTO2703.Data;
using PROGETTO2703.DTO;
using PROGETTO2703.Models;
using System.Collections.Generic;
using System.Linq;
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
            return Ok(await _context.Artisti
                .Include(a => a.Eventi) // Include gli eventi
                .ToListAsync());
        }

        // GET: api/Artisti/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Artista>> GetArtista(int id)
        {
            var artista = await _context.Artisti
                .Include(a => a.Eventi)
                .FirstOrDefaultAsync(a => a.ArtistaId == id);

            if (artista == null) return NotFound();
            return artista;
        }

        // POST: api/Artisti
        [Authorize(Roles = "Amministratore")]
        [HttpPost]
        public async Task<IActionResult> CreaArtista([FromBody] ArtistaDto artistaDto)
        {
            if (artistaDto == null)
            {
                return BadRequest("Dati artista mancanti.");
            }

            var artista = new Artista
            {
                Nome = artistaDto.Nome,
                Genere = artistaDto.Genere,
                Biografia = artistaDto.Biografia,
                Eventi = new List<Evento>()
            };

            // Associa gli eventi solo se presenti nella DTO
            if (artistaDto.Eventi != null && artistaDto.Eventi.Any())
            {
                var eventi = await _context.Eventi
                    .Where(e => artistaDto.Eventi.Contains(e.EventoId))
                    .ToListAsync();

                foreach (var evento in eventi)
                {
                    artista.Eventi.Add(evento);
                }
            }

            _context.Artisti.Add(artista);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArtista), new { id = artista.ArtistaId }, artista);
        }

        // PUT: api/Artisti/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Amministratore")]
        public async Task<IActionResult> Put(int id, [FromBody] ArtistaDto artistaDto)
        {
            var artista = await _context.Artisti
                .Include(a => a.Eventi)
                .FirstOrDefaultAsync(a => a.ArtistaId == id);

            if (artista == null)
            {
                return NotFound();
            }

            artista.Nome = artistaDto.Nome;
            artista.Genere = artistaDto.Genere;
            artista.Biografia = artistaDto.Biografia;

            // Aggiorna gli eventi associati
            if (artistaDto.Eventi != null)
            {
                var eventi = await _context.Eventi
                    .Where(e => artistaDto.Eventi.Contains(e.EventoId))
                    .ToListAsync();

                artista.Eventi = eventi;
            }

            _context.Artisti.Update(artista);
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

