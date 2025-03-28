using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROGETTO2703.Data;
using PROGETTO2703.DTO;
using PROGETTO2703.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PROGETTO2703.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BigliettiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BigliettiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Biglietti (Acquisto)
        [HttpPost]
        [Authorize(Roles = "Utente")]
        public async Task<ActionResult<Biglietto>> AcquistaBiglietto([FromBody] AcquistoBigliettoDto dto)
        {
            // Recupero dell'utente autenticato
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Utente non autenticato.");

            // Verifica se l'evento esiste
            var evento = await _context.Eventi.FindAsync(dto.EventoId);
            if (evento == null)
                return NotFound("Evento non trovato.");

            // Crea il biglietto
            var biglietto = new Biglietto
            {
                EventoId = dto.EventoId,
                UserId = userId,
                DataAcquisto = DateTime.UtcNow
            };

            // Aggiungi biglietti in base a 'Quantita'
            for (int i = 0; i < dto.Quantita; i++)
            {
                _context.Biglietti.Add(new Biglietto
                {
                    EventoId = dto.EventoId,
                    UserId = userId,
                    DataAcquisto = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(biglietto);
        }

        // GET: api/Biglietti (Biglietti dell'utente)
        [HttpGet]
        [Authorize(Roles = "Utente")]
        public async Task<ActionResult<IEnumerable<Biglietto>>> GetBigliettiUtente()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var biglietti = await _context.Biglietti
                .Include(b => b.Evento)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return Ok(biglietti);
        }

        // GET: api/Biglietti/Tutti (solo admin)
        [HttpGet("Tutti")]
        [Authorize(Roles = "Amministratore")]
        public async Task<ActionResult<IEnumerable<Biglietto>>> GetTuttiBiglietti()
        {
            var biglietti = await _context.Biglietti
                .Include(b => b.Evento)
                .Include(b => b.Utente)
                .ToListAsync();
            return Ok(biglietti);
        }
    }
}
