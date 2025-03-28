using System.ComponentModel.DataAnnotations;

namespace PROGETTO2703.Models
{
    public class Artista
    {
        [Key]
        public int ArtistaId { get; set; }

        [Required]
        public string Nome { get; set; }

        public string Genere { get; set; }
        public string Biografia { get; set; }

        // Relazione uno-a-molti: Un artista può avere più eventi
        public ICollection<Evento> Eventi { get; set; }
    }
}
