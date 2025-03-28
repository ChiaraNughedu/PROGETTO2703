using Microsoft.AspNetCore.Identity;
using PROGETTO2703.Models;

namespace PROGETTO2703.Models
{
    public class ApplicationRole : IdentityRole
    {

        public ICollection<ApplicationUserRole> ApplicationUsers { get; set; }
    }
}
