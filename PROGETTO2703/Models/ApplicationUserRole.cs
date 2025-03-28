using Microsoft.AspNetCore.Identity;
using PROGETTO2703.Models;

namespace PROGETTO2703.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {

        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
