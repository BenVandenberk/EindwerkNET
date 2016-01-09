using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UurFac.Models.UurFac;
using UurFac.Models.Repo;

namespace UurFac.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class TariefsController : Controller
    {
        private readonly IUnitOfWork uow;

        public TariefsController(IUnitOfWork uow)
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
                IQueryable<Tarief> tarieven = uow.TariefRepository.AllSorted(jtSorting);
                return Json(new { Result = "OK", Records = tarieven });
            } catch(Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Create(Tarief tarief)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Result = "ERROR", Message = "Kon geen tarief aanmaken: ongeldige waarde" });
            }
            try
            {
                uow.TariefRepository.InsertOrUpdate(tarief);
                uow.saveChanges();
                return Json(new { Result = "OK", Record = tarief });
            } catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon geen tarief aanmaken: " + ex.Message });
            }
        }

        public JsonResult Update(Tarief tarief)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Result = "ERROR", Message = "Kon tarief niet aanpassen: ongeldige waarde" });
            }
            try
            {
                uow.TariefRepository.InsertOrUpdate(tarief);
                uow.saveChanges();
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Kon tarief niet aanpassen: " + ex.Message });
            }
        }

        public JsonResult Delete(int id)
        {
            try
            {
                IEnumerable<UurRegistratieDetail> uurDetails = uow.UurRegistratieDetailRepository.All;
                bool verwijderbaar = true;
                IEnumerator<UurRegistratieDetail> enumerator = uurDetails.GetEnumerator();

                while (verwijderbaar && enumerator.MoveNext())
                {
                    verwijderbaar = enumerator.Current.TariefId != id;
                }
                
                if (!verwijderbaar)
                {
                    return Json(new { Result = "ERROR", Message = "Dit Tarief is niet verwijderbaar omdat het reeds gebruikt is in Uurregistraties" });
                }

                uow.TariefRepository.Delete(id);
                uow.saveChanges();
                return Json(new { Result = "OK" });
            } catch (Exception ex)
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

