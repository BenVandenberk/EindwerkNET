using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UurFac.Models.UurFac;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UurFac.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int GebruikerId { get; set; }
        public virtual Gebruiker Gebruiker { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }


    public class IdentityManager
    {
        private UurFacContext context;

        public IdentityManager(UurFacContext context)
        {
            this.context = context;
        }

        public bool RoleExists(string name)
        {
            var rm = new ApplicationRoleManager(
                new RoleStore<IdentityRole>(context));
            return rm.RoleExists(name);
        }


        public bool CreateRole(string name)
        {
            var rm = new ApplicationRoleManager(
                new RoleStore<IdentityRole>(context));
            var idResult = rm.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }


        public bool CreateUser(ApplicationUser user, string password)
        {
            var um = new ApplicationUserManager(
                new UserStore<ApplicationUser>(context));
            var idResult = um.Create(user, password);
            return idResult.Succeeded;
        }


        public bool AddUserToRole(string userId, string roleName)
        {
            var um = new ApplicationUserManager(
                new UserStore<ApplicationUser>(context));
            var idResult = um.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }


        public void ClearUserRoles(string userId)
        {
            var um = new ApplicationUserManager(
                new UserStore<ApplicationUser>(context));
            var user = um.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();
            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                um.RemoveFromRole(userId, role.RoleId);
            }
        }

      
    }
}