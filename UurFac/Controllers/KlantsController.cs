using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UurFac.Models.UurFac;
using UurFac.Models.Repo;
using UurFac.Models.Service;
using UurFac.Models;
using System.Data.Entity.Validation;

namespace UurFac.Controllers
{
    [Authorize(Roles = "Administrator, Departement Administrator")]
    public class KlantsController : Controller
    {
        private readonly IUnitOfWork uow;
        private IKlantService klantService;
        private IGebruikerService gebruikerService;

        public KlantsController(IUnitOfWork uow, IKlantService klantService, IGebruikerService gebruikerService)
        {
            this.uow = uow;
            this.klantService = klantService;
            this.gebruikerService = gebruikerService;
        }

        [HttpPost]
        public JsonResult List(string jtSorting = null)
        {
            try
            {
                Gebruiker loggedIn = gebruikerService.LoggedIn();
                var klanten = klantService.AlleKlanten(loggedIn).Select(k => new
                {
                    Id = k.Id,
                    Ondernemingsnummer = k.Ondernemingsnummer,
                    Bedrijfsnaam = k.Bedrijfsnaam,
                    Adres = k.Adres,
                    Postcode = k.Postcode,
                    Plaats = k.Plaats
                });
                return Json(new { Result = "OK", Records = sort(klanten, jtSorting) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ListDepartementen(int klantId, string jtSorting = null)
        {   
            try
            {
                var departementen = klantService.Departementen(klantId).Select(d => new
                {
                    Id = d.Id,
                    Code = d.Code,
                    Omschrijving = d.Omschrijving
                });
                return Json(new { Result = "OK", Records = sort(departementen, jtSorting) });
            } catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult MogelijkeDepartementen(int? klantId)
        {
            if (!klantId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen klant geselecteerd" });
            }

            try
            {
                Gebruiker loggedIn = gebruikerService.LoggedIn();
                var departementen = klantService.MogelijkeDepartementenKlantGebruiker(klantId.Value, loggedIn).Select(d => new
                {
                    Value = d.Id,
                    DisplayText = d.Omschrijving
                });
                return Json(new { Result = "OK", Options = departementen });
            } catch(Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }            
        }

        [HttpPost]
        public JsonResult KoppelDepartementAanKlant(int? DepartementId, int? klantId)
        {
            if (!DepartementId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Specifiëer een departement om toe te voegen" });
            }
            if (!klantId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen klant geselecteerd" });
            }

            int depId = DepartementId.Value;

            try
            {
                DepartementKlant koppeling = klantService.KoppelDepartementAanKlant(depId, klantId.Value);
                Departement added = koppeling.Departement;
                return Json(new { Result = "OK", Record = new {Id = added.Id, Code = added.Code, Omschrijving = added.Omschrijving } });
            } catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult OntkoppelDepartementVanKlant(int Id, int klantId)
        {
            try
            {
                Gebruiker loggedIn = gebruikerService.LoggedIn();

                klantService.OntkoppelDepartementVanKlant(Id, klantId, loggedIn);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Create(Klant klant, int? GekozenDepartement)
        {
            if (!GekozenDepartement.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Een klant moet gekoppeld zijn aan minstens één departement" });
            }

            try
            {
                klantService.Nieuw(klant, GekozenDepartement.Value);
                var jsonklant = new { Id = klant.Id, Bedrijfsnaam = klant.Bedrijfsnaam, Ondernemingsnummer = klant.Ondernemingsnummer, Adres = klant.Adres, Postcode = klant.Postcode, Plaats = klant.Plaats };
                return Json(new { Result = "OK" });
            }
            catch (DbEntityValidationException dbEx)
            {
                string message = "";
                foreach(DbEntityValidationResult res in dbEx.EntityValidationErrors)
                {
                    message += res.ValidationErrors.First().ErrorMessage + Environment.NewLine;
                }
                return Json(new { Result = "ERROR", Message = "Kon klant niet aanpassen: " + message });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon klant niet aanpassen: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Update(Klant klant)
        {            
            try
            {
                uow.KlantRepository.InsertOrUpdate(klant);
                uow.saveChanges();                
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon klant niet aanpassen: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen id gespecifieerd om te verwijderen" });
            }            

            try
            {
                Gebruiker loggedIn = gebruikerService.LoggedIn();

                klantService.Delete(id.Value, loggedIn);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ViewResult Index()
        {
            KlantViewModel kvm = klantService.KlantViewModel(gebruikerService.LoggedIn());
            return View(kvm);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                uow.Dispose();
                klantService.Dispose();
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
                case "Ondernemingsnummer":
                    return asc ? list.OrderBy(t => t.Ondernemingsnummer) : list.OrderByDescending(t => t.Ondernemingsnummer);
                case "Bedrijfsnaam":
                    return asc ? list.OrderBy(t => t.Bedrijfsnaam) : list.OrderByDescending(t => t.Bedrijfsnaam);
                case "Postcode":
                    return asc ? list.OrderBy(t => t.Postcode) : list.OrderByDescending(t => t.Postcode);
                case "Adres":
                    return asc ? list.OrderBy(t => t.Adres) : list.OrderByDescending(t => t.Adres);
                case "Plaats":
                    return asc ? list.OrderBy(t => t.Plaats) : list.OrderByDescending(t => t.Plaats);
                case "Code":
                    return asc ? list.OrderBy(t => t.Code) : list.OrderByDescending(t => t.Code);
                case "Omschrijving":
                    return asc ? list.OrderBy(t => t.Omschrijving) : list.OrderByDescending(t => t.Omschrijving);
                default:
                    return list;
            }
        }
    }
}

