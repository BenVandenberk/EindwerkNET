using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UurFac.Models.UurFac;
using UurFac.Models.Repo;
using UurFac.Models.Service;
using UurFac.Models;

namespace UurFac.Controllers
{
    [Authorize(Roles ="Administrator, Departement Administrator")]
    public class GebruikerKlantsController : Controller
    {        
        private IGebruikerKlantService gebruikerKlantService;
        private IGebruikerService gebruikerService;

        public GebruikerKlantsController(IGebruikerKlantService gebruikerKlantService, IGebruikerService gebruikerService)
        {
            this.gebruikerKlantService = gebruikerKlantService;
            this.gebruikerService = gebruikerService;
        }

        [HttpPost]
        public JsonResult ListGebruikers(int? departementId, string jtSorting = null)
        {
            if (!departementId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen departement geselecteerd" });
            }

            int depId = departementId.Value;

            try
            {
                var gebruikers = gebruikerKlantService.GebruikersVan(depId).Select(g => new
                {
                    Id = g.Id,
                    Voornaam = g.Gebruiker.Voornaam,
                    Achternaam = g.Gebruiker.Achternaam,
                    Rol = g.Gebruiker.Rol
                });
                return Json(new { Result = "OK", Records = sort(gebruikers, jtSorting) });
            } catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ListKlanten(int? gebruikerDepId, string jtSorting = null)
        {
            if (!gebruikerDepId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker geselecteerd" });
            }

            int gebDepId = gebruikerDepId.Value;

            try
            {
                var gebruikerKlanten = gebruikerKlantService.KlantenVan(gebDepId).Select(k => new
                {
                    Id = k.Id,
                    Bedrijfsnaam = k.DepartementKlant.Klant.Bedrijfsnaam,
                    Ondernemingsnummer = k.DepartementKlant.Klant.Ondernemingsnummer
                });
                return Json(new { Result = "OK", Records = sort(gebruikerKlanten, jtSorting) });
            } catch(Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }            
        }

        [HttpPost]
        public JsonResult MogelijkeKlantenVoorGebruiker(int? gebruikerDepId)
        {
            if (!gebruikerDepId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker geselecteerd" });
            }

            int gebDepId = gebruikerDepId.Value;

            try
            {
                var departementKlanten = gebruikerKlantService.MogelijkeKlantenVoor(gebDepId).Select(k => new
                {
                    Value = k.Id,
                    DisplayText = k.Klant.Bedrijfsnaam
                });
                return Json(new { Result = "OK", Options = departementKlanten });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult KoppelKlantAanGebruiker(int? gebruikerDepId, int? depKlantId)
        {
            if (!gebruikerDepId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker geselecteerd" });
            }
            if (!depKlantId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen klant geselecteerd" });
            }
            
            try
            {
                GebruikerKlant gk = gebruikerKlantService.KoppelGebruikerAanKlant(gebruikerDepId.Value, depKlantId.Value);
                Klant k = gk.DepartementKlant.Klant;
                return Json(new { Result = "OK", Record = new { Id = gk.Id, Ondernemingsnummer = k.Ondernemingsnummer, Bedrijfsnaam = k.Bedrijfsnaam } });
            } catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }        

        [HttpPost]
        public JsonResult OntkoppelKlantVanGebruiker(int? Id)
        {            
            if (!Id.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen klant geselecteerd" });
            }

            try
            {
                gebruikerKlantService.OntkoppelGebruikerVanKlant(Id.Value);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ViewResult Index()
        {
            Gebruiker loggedIn = gebruikerService.LoggedIn();
            GebruikerKlantViewModel gkvm = gebruikerKlantService.GebruikerKlantViewModel(loggedIn);
            return View(gkvm);
        }       

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                gebruikerKlantService.Dispose();
                gebruikerService.Dispose();
            }
            base.Dispose(disposing);
        }

        private IEnumerable<dynamic> sort(IEnumerable<dynamic> list, string jtSortOrder)
        {
            if (jtSortOrder == null)
            {
                return list;
            }

            string[] propOrder = jtSortOrder.Split(' ');
            string prop = propOrder[0];
            bool asc = propOrder[1] == "ASC";

            switch (prop)
            {
                case "Voornaam":
                    return asc? list.OrderBy(t => t.Voornaam) : list.OrderByDescending(t => t.Voornaam);
                case "Achternaam":
                    return asc? list.OrderBy(t => t.Achternaam) : list.OrderByDescending(t => t.Achternaam);                    
                case "Rol":
                    return asc? list.OrderBy(t => t.Rol) : list.OrderByDescending(t => t.Rol);
                case "Ondernemingsnummer":
                    return asc ? list.OrderBy(t => t.Ondernemingsnummer) : list.OrderByDescending(t => t.Ondernemingsnummer);
                case "Bedrijfsnaam":
                    return asc ? list.OrderBy(t => t.Bedrijfsnaam) : list.OrderByDescending(t => t.Bedrijfsnaam);
                default:
                    return list;
            }
        }
    }
}

