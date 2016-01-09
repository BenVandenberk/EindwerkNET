using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UurFac.Models.UurFac;
using UurFac.Models.Repo;

namespace UurFac.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class DepartementsController : Controller
    {
        private readonly IUnitOfWork uow;

        public DepartementsController(IUnitOfWork uow)
        {
            this.uow = uow;
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
                var departementen = uow.DepartementRepository.AllSorted(jtSorting).Select(d => new {Id = d.Id, Code = d.Code, Omschrijving = d.Omschrijving });
                return Json(new { Result = "OK", Records = departementen });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Create(Departement departement)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Result = "ERROR", Message = "Kon geen departement aanmaken: ongeldige waarde" });
            }
            try
            {
                uow.DepartementRepository.InsertOrUpdate(departement);
                uow.saveChanges();
                return Json(new { Result = "OK", Record = departement });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon geen departement aanmaken: " + ex.Message });
            }
        }

        public JsonResult Update(Departement departement)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Result = "ERROR", Message = "Kon departement niet aanpassen: ongeldige waarde" });
            }
            try
            {
                uow.DepartementRepository.InsertOrUpdate(departement);
                uow.saveChanges();
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon departement niet aanpassen: " + ex.Message });
            }
        }

        public JsonResult Delete(int id)
        {
            Departement departement = uow.DepartementRepository.Find(id);

            if (departement == null)
            {
                return Json(new { Result = "ERROR", Message = "Ongeldige Id" });
            }
            if (!departement.Verwijderbaar())
            {
                return Json(new { Result = "ERROR", Message = "Departement is niet verwijderbaar omdat er reeds klanten en/of gebruikers aan gekoppeld zijn" });
            }

            try
            {     
                uow.DepartementRepository.Delete(id);
                uow.saveChanges();
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
            }
            base.Dispose(disposing);
        }
    }
}

