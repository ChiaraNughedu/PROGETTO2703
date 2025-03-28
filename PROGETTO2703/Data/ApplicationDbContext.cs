using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROGETTO2703.Models;

namespace PROGETTO2703.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
        ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Artista> Artisti { get; set; }
        public DbSet<Evento> Eventi { get; set; }
        public DbSet<Biglietto> Biglietti { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurazione per la tabella di join custom (ApplicationUserRole)
            builder.Entity<ApplicationUserRole>(entity =>
            {
                // NON chiamare HasKey qui, perché la chiave è già definita in IdentityUserRole<string>

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);
            });



            // Relazione tra Evento e Artista
            builder.Entity<Evento>()
                .HasOne(e => e.Artista)
                .WithMany(a => a.Eventi)
                .HasForeignKey(e => e.ArtistaId);

            // Relazione tra Biglietto e Evento
            builder.Entity<Biglietto>()
                .HasOne(b => b.Evento)
                .WithMany(e => e.Biglietti)
                .HasForeignKey(b => b.EventoId);

            // Relazione tra Biglietto e ApplicationUser
            builder.Entity<Biglietto>()
                .HasOne(b => b.Utente)
                .WithMany(u => u.Biglietti)
                .HasForeignKey(b => b.UserId);
        }
    }
}

