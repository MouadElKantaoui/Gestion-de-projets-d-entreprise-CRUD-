using Microsoft.EntityFrameworkCore;
using ProjetFinal.Models; // adapte le namespace

namespace ProjetFinal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Entreprise> Entreprises => Set<Entreprise>();
        public DbSet<Adresse> Adresses => Set<Adresse>();
        public DbSet<Projet> Projets => Set<Projet>();
        public DbSet<Developpeur> Developpeurs => Set<Developpeur>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1-1 Entreprise <-> Adresse
            modelBuilder.Entity<Entreprise>()
                .HasOne(e => e.Adresse)
                .WithOne(a => a.Entreprise)
                .HasForeignKey<Entreprise>(e => e.AdresseId)
                .OnDelete(DeleteBehavior.NoAction); // pas de cascade

            // 1-N Entreprise -> Projets
            modelBuilder.Entity<Projet>()
                .HasOne(p => p.Entreprise)
                .WithMany(e => e.Projets)
                .HasForeignKey(p => p.EntrepriseId)
                .OnDelete(DeleteBehavior.NoAction); // pas de cascade

            // 1-N Entreprise -> Développeurs
            modelBuilder.Entity<Developpeur>()
                .HasOne(d => d.Entreprise)
                .WithMany(e => e.Developpeurs)
                .HasForeignKey(d => d.EntrepriseId)
                .OnDelete(DeleteBehavior.NoAction); // pas de cascade

            // N-N Projet <-> Développeur avec table de jonction nommée
            modelBuilder.Entity<Projet>()
                .HasMany(p => p.Developpeurs)
                .WithMany(d => d.Projets)
                .UsingEntity<Dictionary<string, object>>(
                    "DeveloppeurProjet",
                    // FK vers Developpeur : cascade OK
                    j => j.HasOne<Developpeur>()
                          .WithMany()
                          .HasForeignKey("DeveloppeursId")
                          .OnDelete(DeleteBehavior.Cascade),
                    // FK vers Projet : pas de cascade pour éviter les chemins multiples
                    j => j.HasOne<Projet>()
                          .WithMany()
                          .HasForeignKey("ProjetsId")
                          .OnDelete(DeleteBehavior.NoAction)
                );
        }
    }
}
