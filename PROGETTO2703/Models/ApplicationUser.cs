using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using PROGETTO2703.Models;

namespace PROGETTO2703.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public string Surname { get; set; }
        [Required]
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public DateTime DeletionDate { get; set; }

        public ICollection<ApplicationUserRole> ApplicationRoles { get; set; }
    }
}
