using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PROGETTO2703.DTO
{
    public class ArtistaDto
    {
        [Required]
        public string Nome { get; set; }

        public string? Genere { get; set; }
        public string? Biografia { get; set; }

        // Lista di ID degli eventi associati all'artista
        public List<int>? Eventi { get; set; }
    }
}

