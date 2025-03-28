using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PROGETTO2703.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
        public string? Cognome { get; set; }
        public DateTime DataRegistrazione { get; set; } = DateTime.UtcNow;

        // Usa la classe custom per la join tra utente e ruolo
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        public ICollection<Biglietto> Biglietti { get; set; } = new List<Biglietto>();
    }
}

