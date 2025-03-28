using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace PROGETTO2703.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Descrizione { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
}
