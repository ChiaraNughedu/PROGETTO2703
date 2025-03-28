using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROGETTO2703.Models
{
    public class Biglietto
    {
        [Key]
        public int BigliettoId { get; set; }

        // Relazione molti-a-uno con Evento
        [ForeignKey("Evento")]
        public int EventoId { get; set; }
        public Evento Evento { get; set; }

        // Relazione molti-a-uno con ApplicationUser
        [ForeignKey("Utente")]
        public string UserId { get; set; }
        public ApplicationUser Utente { get; set; }

        public DateTime DataAcquisto { get; set; } = DateTime.UtcNow;
    }
}

