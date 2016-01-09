using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using UurFac.Models.Repo;
using UurFac.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using UurFac.Controllers;
using UurFac.Models.Service;
using System.Web;
using Microsoft.Owin.Security;

namespace UurFac
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IGebruikerService, GebruikerService>();
            container.RegisterType<IKlantService, KlantService>();
            container.RegisterType<IGebruikerKlantService, GebruikerKlantService>();
            container.RegisterType<IUurRegistratieService, UurRegistratieService>();
            container.RegisterType<IFactuurService, FactuurService>();

            container.RegisterType<DbContext, UurFacContext>(new HierarchicalLifetimeManager());
            container.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<AccountController>(new InjectionConstructor(new UnitOfWork()));
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(o => HttpContext.Current.GetOwinContext().Authentication));


            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}