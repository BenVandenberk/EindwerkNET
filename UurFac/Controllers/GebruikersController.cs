using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UurFac.Models.UurFac;
using UurFac.Models.Repo;
using UurFac.Models;
using UurFac.Models.Service;

namespace UurFac.Controllers
{
    [Authorize(Roles = "Administrator")]    
    public class GebruikersController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly IGebruikerService gebruikerService;

        public GebruikersController(IUnitOfWork uow, IGebruikerService gebruikerService)
        {
            this.uow = uow;
            this.gebruikerService = gebruikerService;
        }
        
        public ViewResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public JsonResult List(string jtSorting = null)
        {
            try
            {
                var gebruikers = uow.GebruikerRepository.AllSorted(jtSorting).Select(x => new {Id = x.Id, Achternaam = x.Achternaam, Voornaam = x.Voornaam, Rol = x.Rol, Login = x.Login });            
                return Json(new { Result = "OK", Records = gebruikers });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        
        [HttpPost]
        public JsonResult Create(Gebruiker gebruiker)
        {           
            try
            {
                gebruikerService.Nieuw(gebruiker);
                var jsonGebruiker = new { Id = gebruiker.Id, Achternaam = gebruiker.Achternaam, Voornaam = gebruiker.Voornaam, Rol = gebruiker.Rol, Login = gebruiker.Login };
                return Json(new { Result = "OK", Record = jsonGebruiker });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon geen gebruiker aanmaken: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Update(Gebruiker gebruiker)
        {           
            try
            {
                gebruikerService.Update(gebruiker);                
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon gebruiker niet aanpassen: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker geselecteerd" });
            }

            try
            {
                gebruikerService.Delete(id.Value);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ListDepartementen(int? gebruikerId)
        {
            if (!gebruikerId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker geselecteerd" });
            }

            Gebruiker gebruiker = uow.GebruikerRepository.Find(gebruikerId.Value);
            if (gebruiker != null)
            {
                var departementen = gebruiker.Departementen().Select(d => new {
                    Id = d.Id,
                    Code = d.Code,
                    Omschrijving = d.Omschrijving
                });
                return Json(new { Result = "OK", Records = departementen });
            } else
            {
                return Json(new { Result = "ERROR", Message = "Gebruiker niet gevonden" });
            }
        }

        [HttpPost]
        public JsonResult ListMogelijkeDepartementen(int? gebruikerId)
        {
            if (!gebruikerId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker geselecteerd" });
            }

            try
            {
                var mogelijkeDepartementen = gebruikerService.ListMogelijkeDepartementen(gebruikerId.Value).Select(d => new {
                    DisplayText = d.Omschrijving,
                    Value = d.Id
                });
                return Json(new { Result = "OK", Options = mogelijkeDepartementen });
            } catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }            
        }

        [HttpPost]
        public JsonResult KoppelDepartement(int? gebruikerId, int? DepartementId)
        {
            if (!gebruikerId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker geselecteerd" });
            }
            if (!DepartementId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen departement geselecteerd" });
            }

            try {
                Departement departement = gebruikerService.Koppel(gebruikerId.Value, DepartementId.Value);
                return Json(new { Result = "OK", Record = new {
                    Code = departement.Code,
                    Omschrijving = departement.Omschrijving,
                    Id = departement.Id
                }});
            } catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult OntkoppelDepartement(int? gebruikerId, int? Id)
        {
            if (!gebruikerId.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen gebruiker geselecteerd" });
            }
            if (!Id.HasValue)
            {
                return Json(new { Result = "ERROR", Message = "Geen departement geselecteerd" });
            }

            try
            {
                gebruikerService.Ontkoppel(gebruikerId.Value, Id.Value);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                uow.Dispose();
                gebruikerService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

