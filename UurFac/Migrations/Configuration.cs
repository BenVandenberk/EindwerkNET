namespace UurFac.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;
    using Models.UurFac;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models.Service;
    using Models.Repo;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<UurFac.Models.UurFacContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "UurFac.Models.UurFacContext";
        }

        protected override void Seed(UurFacContext context)
        {
            //IdentityManager iM = new IdentityManager(context);
            //if (!iM.RoleExists("Administrator"))
            //{
            //    iM.CreateRole("Administrator");
            //}
            //if (!iM.RoleExists("User"))
            //{
            //    iM.CreateRole("User");
            //}
            //if (!iM.RoleExists("Departement Administrator"))
            //{
            //    iM.CreateRole("Departement Administrator");
            //}

            //bool adminBestaat = context.Gebruikers.Any(g => g.Login == "admin");

            //UnitOfWork uow = new UnitOfWork(); ;
            //GebruikerService gebruikerService = new GebruikerService(uow);

            //if (!adminBestaat) {
            //    Gebruiker admin = new Gebruiker()
            //    {
            //        Login = "admin",
            //        Achternaam = "Vandenberk",
            //        Voornaam = "Ben",
            //        Rol = Rol.ADMINISTRATOR,
            //        Email = "Ben.Vandenberk@hotmail.com",
            //        Gsm = "0499/999999",
            //        Tel = "016/222222"
            //    };
            //    gebruikerService.Nieuw(admin);
            //}
        }
    }
}
