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
    [Authorize]
    public class UurRegistratiesController : Controller
    {        
        private IUurRegistratieService uurRegistratieService;
        private IGebruikerService gebruikerService;

        public UurRegistratiesController(IUurRegistratieService uurRegistratieService, IGebruikerService gebruikerService)
        {
            this.uurRegistratieService = uurRegistratieService;
            this.gebruikerService = gebruikerService;
        }

        public ViewResult Index(int? gebruikerKlantId, int? gebruikerDepartementId, int? departementId, bool? gefactureerd)
        {
            if (Request.HttpMethod.ToLower() == "post")
            {
                ViewBag.GebruikerKlantId = gebruikerKlantId.Value;
                ViewBag.GebruikerDepartementId = gebruikerDepartementId.Value;
                ViewBag.DepartementId = departementId.Value;
            }

            bool fac = gefactureerd.HasValue ? gefactureerd.Value : false;

            Gebruiker loggedIn = gebruikerService.LoggedIn();
            return View(uurRegistratieService.UurRegistratieViewModel(loggedIn, fac));
        }

        [HttpPost]
        public JsonResult ListGebruikers(int? departementId)
        {
            if (!departementId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen departement geselecteerd" });
            }

            int depId = departementId.Value;
            Gebruiker loggedIn = gebruikerService.LoggedIn();

            try
            {
                var gebruikerDepartementen = uurRegistratieService.alleGebruikerDepartementenVoor(loggedIn, depId).Select(gd => new
                {
                    Id = gd.Id,
                    Gebruiker = String.Format("{0} {1}", gd.Gebruiker.Voornaam, gd.Gebruiker.Achternaam)
                });
                return Json(new { Result = "OK", Records = gebruikerDepartementen });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ListGebruikerKlanten(int? gebruikerDepartementId, string jtSorting = null)
        {
            if (!gebruikerDepartementId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker en/of departement geselecteerd" });
            }

            int gebDepId = gebruikerDepartementId.Value;           

            try
            {
                var gebruikerKlanten = uurRegistratieService.alleGebruikerKlantenVoor(gebDepId).Select(gk => new
                {
                    Id = gk.Id,
                    Ondernemingsnummer = gk.Klant().Ondernemingsnummer,
                    Bedrijfsnaam = gk.Klant().Bedrijfsnaam
                });
                return Json(new { Result = "OK", Records = sort(gebruikerKlanten, jtSorting) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ListUurRegistraties(int? gebruikerKlantId, bool gefactureerd, string jtSorting = null)
        {
            if (!gebruikerKlantId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker en/of klant geselecteerd" });
            }

            int gebKlantId = gebruikerKlantId.Value;

            try
            {
                var uurRegistraties = uurRegistratieService.alleUurRegistratiesVoor(gebKlantId, gefactureerd).Select(uur => new
                {
                    Id = uur.Id,
                    Titel = uur.Titel,
                    Omschrijving = uur.Omschrijving
                });
                return Json(new { Result = "OK", Records = sort(uurRegistraties, jtSorting) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult NieuweUurRegistratie(int? gebruikerKlantId, UurRegistratie uurRegistratie)
        {
            if (!gebruikerKlantId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker en/of klant geselecteerd" });
            }

            int gebKlantId = gebruikerKlantId.Value;

            try
            {
                uurRegistratieService.Nieuw(gebKlantId, uurRegistratie);
                return Json(new { Result = "OK", Record = new { Id = uurRegistratie.Id, Titel = uurRegistratie.Titel, Omschrijving = uurRegistratie.Omschrijving } });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Update(UurRegistratie uurRegistratie)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Result = "ERROR", Message = "Kon uurregistratie niet aanpassen: ongeldige waarde" });
            }
            try
            {
                uurRegistratieService.Update(uurRegistratie);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon uurregistratie niet aanpassen: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(UurRegistratie uurRegistratie)
        {            
            try
            {
                uurRegistratieService.Delete(uurRegistratie);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon uurregistratie niet verwijderen: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Details(int? uurRegistratieId, bool? gefactureerd)
        {
            if (!uurRegistratieId.HasValue)
            {
                return new EmptyResult();
            }

            bool fac = gefactureerd.HasValue ? gefactureerd.Value : false;

            UurRegistratieDetailViewModel urdvm = uurRegistratieService.UurRegistratieDetailViewModel(uurRegistratieId.Value, fac);

            if (urdvm != null) {
                ViewBag.Id = uurRegistratieId.Value; 
                return View(urdvm);
            } else
            {
                return new EmptyResult();
            }
        }

        [HttpPost]
        public JsonResult ListDetails(int? uurRegistratieId)
        {
            if (!uurRegistratieId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen uurregistratie geselecteerd" });
            }

            int uurRegId = uurRegistratieId.Value;

            try
            {
                var uurRegistratieDetails = uurRegistratieService.alleDetailsVoor(uurRegId).Select(det => new
                {
                    Id = det.Id,
                    StartTijd = det.StartTijd,
                    EindTijd = det.EindTijd,
                    TypeWerk = det.Tarief.TypeWerk,
                    UurWaarde = det.Tarief.TariefUurWaarde,
                    TeFactureren = det.TeFactureren
                });
                return Json(new { Result = "OK", Records = uurRegistratieDetails });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult NieuwUurRegistratieDetail(int? uurRegistratieId, UurRegistratieDetail uurRegistratieDetail, int? TariefId)
        {
            if (!uurRegistratieId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen uurregistratie geselecteerd" });
            }
            if (!TariefId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen tarief gespecifieerd" });
            }

            int uurRegId = uurRegistratieId.Value;
            int tarId = TariefId.Value;

            try
            {
                uurRegistratieService.NieuwDetail(uurRegId, uurRegistratieDetail, tarId);
                return Json(new { Result = "OK", Record = new {
                    Id = uurRegistratieDetail.Id,
                    StartTijd = uurRegistratieDetail.StartTijd,
                    EindTijd = uurRegistratieDetail.EindTijd,
                    TeFactureren = uurRegistratieDetail.TeFactureren,
                    TypeWerk = uurRegistratieDetail.Tarief.TypeWerk,
                    UurWaarde = uurRegistratieDetail.Tarief.TariefUurWaarde,
                    TariefId = tarId
                } });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteDetail(UurRegistratieDetail uurRegistratieDetail)
        {
            try
            {
                uurRegistratieService.DeleteDetail(uurRegistratieDetail.Id);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon uurregistratiedetail niet verwijderen: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateDetail(UurRegistratieDetail uurRegistratieDetail)
        {
            try
            {
                uurRegistratieService.UpdateDetail(uurRegistratieDetail);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon uurregistratiedetail niet aanpassen: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AlleTarieven()
        {
            try
            {
                var tarieven = uurRegistratieService.alleTarieven().Select(t => new
                {
                    Value = t.Id,
                    DisplayText = t.TypeWerk
                });
                return Json(new { Result = "OK", Options = tarieven });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                uurRegistratieService.Dispose();
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
                case "Titel":
                    return asc ? list.OrderBy(t => t.Titel) : list.OrderByDescending(t => t.Titel);
                case "Omschrijving":
                    return asc ? list.OrderBy(t => t.Omschrijving) : list.OrderByDescending(t => t.Omschrijving);
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

