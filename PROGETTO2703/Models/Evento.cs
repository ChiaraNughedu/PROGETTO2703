using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROGETTO2703.Models
{
    public class Evento
    {
        [Key]
        public int EventoId { get; set; }

        [Required]
        public string Titolo { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        public string Luogo { get; set; }

        // Relazione molti-a-uno con Artista
        [ForeignKey("Artista")]
        public int ArtistaId { get; set; }
        public Artista Artista { get; set; }

        // Relazione uno-a-molti con Biglietto
        public ICollection<Biglietto> Biglietti { get; set; }
    }
}
