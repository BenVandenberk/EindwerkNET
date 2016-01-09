using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UurFac.Models.UurFac;
using UurFac.Models.Repo;
using UurFac.Models.Service;

namespace UurFac.Controllers
{
    [Authorize(Roles = "Administrator, Departement Administrator")]
    public class FactuursController : Controller
    {        
        private IFactuurService factuurService;
        private IGebruikerService gebruikerService;

        public FactuursController(IFactuurService factuurService, IGebruikerService gebruikerService)
        {            
            this.factuurService = factuurService;
            this.gebruikerService = gebruikerService;
        }
        
        [HttpPost]
        public JsonResult AlleDepartementen()
        {
            Gebruiker loggedIn = gebruikerService.LoggedIn();

            var departementen = factuurService.AlleMogelijkeDepartementen(loggedIn).Select(d => new
            {
                Id = d.Id,
                Omschrijving = d.Omschrijving
            });

            return Json(departementen);
        }     
        
        [HttpPost]
        public JsonResult AlleDepartementKlanten(int? departementId)
        {
            if (!departementId.HasValue)
            {
                return Json(new { ERROR = "Geen departementId gespecifieerd" });
            }

            var departementKlanten = factuurService.AlleDepartementKlantenVoor(departementId.Value).Select(dk => new
            {
                Id = dk.Id,
                Bedrijfsnaam = dk.Klant.Bedrijfsnaam
            });

            return Json(departementKlanten);
        } 

        [HttpPost]
        public JsonResult ZoekUurRegistraties(int? departementKlantId, DateTime? van, DateTime? tot)
        {
            if (!departementKlantId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen departement / klant geselecteerd" });
            }            

            int depklantId = departementKlantId.Value;                        

            try
            {
                var uurRegistraties = factuurService.ZoekUurRegistraties(depklantId, van, tot).Select(u => new
                {
                    Id = u.Id,
                    Titel = u.Titel,
                    Omschrijving = u.Omschrijving,
                    Gebruiker = u.GebruikerKlant.GebruikerDepartement.Gebruiker.volledigeNaam()
                });
                return Json(new { Result = "OK", Records = uurRegistraties });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Factureer(string uurRegistratieIds, int? departementKlantId)
        {
            if (uurRegistratieIds == null || !departementKlantId.HasValue)
            {
                ViewBag.Error = "Ongeldige uurregistraties / departement / klant";
                return View("Create");
            }

            string[] ids = uurRegistratieIds.Split(',');

            if (ids.Length == 0)
            {
                ViewBag.Error = "Ongeldige uurregistraties";
                return View("Create");
            }

            try {
                int[] intIds = ids.Select(st => Int32.Parse(st)).ToArray();
                Factuur factuur = factuurService.Factureer(intIds, departementKlantId.Value);

                return View("Details", factuurService.FactuurViewModel(factuur.Id, true));
            } catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Create");
            }
        }

        [HttpPost]
        public ActionResult Annuleer(int? factuurId)
        {
            if (!factuurId.HasValue)
            {
                ViewBag.Error = "Geen geldige factuurId";
                return View("Create");
            }

            try
            {
                factuurService.AnnuleerFactuur(factuurId.Value);
                return View("Create");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Create");
            }
        }

        [HttpPost]
        public JsonResult ListFacturen(int? departementId, string jtSorting = null)
        {
            if (!departementId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen departement geselecteerd" });
            }

            try
            {
                var facturen = factuurService.AlleFacturenVoor(departementId.Value).Select(f => new
                {
                    Id = f.Id,
                    Factuurnummer = f.FactuurNummer,
                    Factuurdatum = f.FactuurDatum,
                    Totaal = f.Totaal,
                    Klant = f.DepartementKlant.Klant.Bedrijfsnaam,
                    Departement = f.DepartementKlant.Departement.Omschrijving
                });
                return Json(new { Result = "OK", Records = sort(facturen, jtSorting) });
            } catch(Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ViewResult Details(int id)
        {
            return View(factuurService.FactuurViewModel(id, false));
        }        

        public ActionResult Create()
        {			
            return View();
        }

        public ActionResult Index(int? departementId)
        {
            if(Request.HttpMethod.ToLower() == "post")
            {
                ViewBag.DepId = departementId.Value;
            }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                gebruikerService.Dispose();
                factuurService.Dispose();
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
                case "Factuurnummer":
                    return asc ? list.OrderBy(t => t.Factuurnummer) : list.OrderByDescending(t => t.Factuurnummer);
                case "Factuurdatum":
                    return asc ? list.OrderBy(t => t.Factuurdatum) : list.OrderByDescending(t => t.Factuurdatum);
                case "Totaal":
                    return asc ? list.OrderBy(t => t.Totaal) : list.OrderByDescending(t => t.Totaal);
                case "Klant":
                    return asc ? list.OrderBy(t => t.Klant) : list.OrderByDescending(t => t.Klant);
                case "Departement":
                    return asc ? list.OrderBy(t => t.Departement) : list.OrderByDescending(t => t.Departement);
                default:
                    return list;
            }
        }
    }
}

