namespace UurFac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DepartementKlant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepartementId = c.Int(nullable: false),
                        KlantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departement", t => t.DepartementId, cascadeDelete: true)
                .ForeignKey("dbo.Klant", t => t.KlantId, cascadeDelete: true)
                .Index(t => t.DepartementId)
                .Index(t => t.KlantId);
            
            CreateTable(
                "dbo.Departement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Omschrijving = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GebruikerDepartement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AdministratorVan = c.Boolean(nullable: false),
                        GebruikerId = c.Int(nullable: false),
                        DepartementId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departement", t => t.DepartementId, cascadeDelete: true)
                .ForeignKey("dbo.Gebruiker", t => t.GebruikerId, cascadeDelete: true)
                .Index(t => t.GebruikerId)
                .Index(t => t.DepartementId);
            
            CreateTable(
                "dbo.Gebruiker",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Voornaam = c.String(),
                        Achternaam = c.String(),
                        Email = c.String(),
                        Tel = c.String(),
                        Gsm = c.String(),
                        Rol = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        GebruikerId = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.GebruikerKlant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GebruikerDepartementId = c.Int(nullable: false),
                        DepartementKlantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DepartementKlant", t => t.DepartementKlantId)
                .ForeignKey("dbo.GebruikerDepartement", t => t.GebruikerDepartementId)
                .Index(t => t.GebruikerDepartementId)
                .Index(t => t.DepartementKlantId);
            
            CreateTable(
                "dbo.UurRegistratie",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Titel = c.String(),
                        Omschrijving = c.String(),
                        GebruikerKlantId = c.Int(nullable: false),
                        FactuurDetailId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GebruikerKlant", t => t.GebruikerKlantId, cascadeDelete: true)
                .Index(t => t.GebruikerKlantId);
            
            CreateTable(
                "dbo.FactuurDetail",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Omschrijving = c.String(),
                        LijnWaarde = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FactuurId = c.Int(nullable: false),
                        UurRegistratieId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Factuur", t => t.FactuurId, cascadeDelete: true)
                .ForeignKey("dbo.UurRegistratie", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.FactuurId);
            
            CreateTable(
                "dbo.Factuur",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FactuurJaar = c.Int(nullable: false),
                        FactuurNummer = c.String(),
                        FactuurDatum = c.DateTime(nullable: false),
                        DepartementKlantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DepartementKlant", t => t.DepartementKlantId, cascadeDelete: true)
                .Index(t => t.DepartementKlantId);
            
            CreateTable(
                "dbo.UurRegistratieDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTijd = c.DateTime(nullable: false),
                        EindTijd = c.DateTime(nullable: false),
                        TeFactureren = c.Boolean(nullable: false),
                        UurRegistratieId = c.Int(nullable: false),
                        TariefId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tarief", t => t.TariefId, cascadeDelete: true)
                .ForeignKey("dbo.UurRegistratie", t => t.UurRegistratieId, cascadeDelete: true)
                .Index(t => t.UurRegistratieId)
                .Index(t => t.TariefId);
            
            CreateTable(
                "dbo.Tarief",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeWerk = c.String(),
                        GeldigVanaf = c.DateTime(nullable: false),
                        TariefUurWaarde = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Klant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ondernemingsnummer = c.String(),
                        Bedrijfsnaam = c.String(),
                        Adres = c.String(),
                        Postcode = c.String(),
                        Plaats = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DepartementKlant", "KlantId", "dbo.Klant");
            DropForeignKey("dbo.UurRegistratieDetail", "UurRegistratieId", "dbo.UurRegistratie");
            DropForeignKey("dbo.UurRegistratieDetail", "TariefId", "dbo.Tarief");
            DropForeignKey("dbo.UurRegistratie", "GebruikerKlantId", "dbo.GebruikerKlant");
            DropForeignKey("dbo.FactuurDetail", "Id", "dbo.UurRegistratie");
            DropForeignKey("dbo.FactuurDetail", "FactuurId", "dbo.Factuur");
            DropForeignKey("dbo.Factuur", "DepartementKlantId", "dbo.DepartementKlant");
            DropForeignKey("dbo.GebruikerKlant", "GebruikerDepartementId", "dbo.GebruikerDepartement");
            DropForeignKey("dbo.GebruikerKlant", "DepartementKlantId", "dbo.DepartementKlant");
            DropForeignKey("dbo.Gebruiker", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GebruikerDepartement", "GebruikerId", "dbo.Gebruiker");
            DropForeignKey("dbo.GebruikerDepartement", "DepartementId", "dbo.Departement");
            DropForeignKey("dbo.DepartementKlant", "DepartementId", "dbo.Departement");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.UurRegistratieDetail", new[] { "TariefId" });
            DropIndex("dbo.UurRegistratieDetail", new[] { "UurRegistratieId" });
            DropIndex("dbo.Factuur", new[] { "DepartementKlantId" });
            DropIndex("dbo.FactuurDetail", new[] { "FactuurId" });
            DropIndex("dbo.FactuurDetail", new[] { "Id" });
            DropIndex("dbo.UurRegistratie", new[] { "GebruikerKlantId" });
            DropIndex("dbo.GebruikerKlant", new[] { "DepartementKlantId" });
            DropIndex("dbo.GebruikerKlant", new[] { "GebruikerDepartementId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Gebruiker", new[] { "User_Id" });
            DropIndex("dbo.GebruikerDepartement", new[] { "DepartementId" });
            DropIndex("dbo.GebruikerDepartement", new[] { "GebruikerId" });
            DropIndex("dbo.DepartementKlant", new[] { "KlantId" });
            DropIndex("dbo.DepartementKlant", new[] { "DepartementId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Klant");
            DropTable("dbo.Tarief");
            DropTable("dbo.UurRegistratieDetail");
            DropTable("dbo.Factuur");
            DropTable("dbo.FactuurDetail");
            DropTable("dbo.UurRegistratie");
            DropTable("dbo.GebruikerKlant");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Gebruiker");
            DropTable("dbo.GebruikerDepartement");
            DropTable("dbo.Departement");
            DropTable("dbo.DepartementKlant");
        }
    }
}
