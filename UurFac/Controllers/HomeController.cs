﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UurFac.Models.UurFac;

namespace UurFac.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {         
            return View();
        }
    }
}