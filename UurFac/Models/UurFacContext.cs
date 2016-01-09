using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models
{
    public class UurFacContext : IdentityDbContext<ApplicationUser>
    {

        public UurFacContext() : base("name=UurFacDb")
        {
            Database.SetInitializer<UurFacContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<GebruikerKlant>().HasRequired(g => g.GebruikerDepartement).WithMany(gebr => gebr.GebruikerKlanten).WillCascadeOnDelete(false);
            modelBuilder.Entity<GebruikerKlant>().HasRequired(g => g.DepartementKlant).WithMany(depkl => depkl.GebruikerKlanten).WillCascadeOnDelete(false);

            modelBuilder.Entity<UurRegistratie>().HasOptional(u => u.FactuurDetail).WithRequired(f => f.UurRegistratie);
        }

        protected override DbEntityValidationResult ValidateEntity(System.Data.Entity.Infrastructure.DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            if (entityEntry.Entity is Klant && (entityEntry.State == EntityState.Added || entityEntry.State == EntityState.Modified))
            {
                //Check of Ondernemingsnummer al bestaat
                Klant klant = (Klant)entityEntry.Entity;
                string nieuwNummer = klant.Ondernemingsnummer;
                bool bestaatAl = Klants.Where(k => k.Id != klant.Id)
                                       .Any(k => k.Ondernemingsnummer == nieuwNummer);

                if (bestaatAl)
                {
                    var list = new List<DbValidationError>();
                    list.Add(new DbValidationError("Ondernemingsnummer", "Dit ondernemingsnummer bestaat al"));

                    return new DbEntityValidationResult(entityEntry, list);
                }
            }
            return base.ValidateEntity(entityEntry, items);
        }

        public virtual DbSet<Gebruiker> Gebruikers { get; set; }       
        public virtual DbSet<FactuurDetail> FactuurDetails { get; set; }        
        public virtual DbSet<UurRegistratie> UurRegistraties { get; set; }
        public virtual DbSet<UurRegistratieDetail> UurRegistratieDetails { get; set; }
        public DbSet<Klant> Klants { get; set; }
        public DbSet<Departement> Departements { get; set; }
        public DbSet<GebruikerKlant> GebruikerKlants { get; set; }
        public DbSet<Factuur> Factuurs { get; set; }
        public DbSet<Tarief> Tariefs { get; set; }
        public DbSet<DepartementKlant> DepartementKlants { get; set; }
        public DbSet<GebruikerDepartement> GebruikerDepartements { get; set; }

        public static UurFacContext Create()
        {
            return new UurFacContext();
        }        
    }
}