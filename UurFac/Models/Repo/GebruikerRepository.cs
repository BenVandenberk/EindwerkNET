using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class GebruikerRepository : IGebruikerRepository
    {
        private UurFacContext context;

        public GebruikerRepository()
        {
            context = new UurFacContext();
        }

        public GebruikerRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<Gebruiker> All
        {
            get { return context.Gebruikers; }
        }

        public IQueryable<Gebruiker> AllSorted(string jtSortOrder)
        {
            if (jtSortOrder == null)
            {
                return All;
            }

            string[] propOrder = jtSortOrder.Split(' ');
            string prop = propOrder[0];
            bool asc = propOrder[1] == "ASC";

            switch (prop)
            {
                case "Login":
                    return asc ? All.OrderBy(t => t.Login) : All.OrderByDescending(t => t.Login);
                case "Voornaam":
                    return asc ? All.OrderBy(t => t.Voornaam) : All.OrderByDescending(t => t.Voornaam);
                case "Achternaam":
                    return asc ? All.OrderBy(t => t.Achternaam) : All.OrderByDescending(t => t.Achternaam);
                case "Rol":
                    return asc ? All.OrderBy(t => t.Rol) : All.OrderByDescending(t => t.Rol);
                default:
                    return All;
            }
        }

        public IQueryable<Gebruiker> AllIncluding(params Expression<Func<Gebruiker, object>>[] includeProperties)
        {
            IQueryable<Gebruiker> query = context.Gebruikers;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Gebruiker Find(int id)
        {
            return context.Gebruikers.Find(id);
        }

        public void InsertOrUpdate(Gebruiker gebruiker)
        {
            if (gebruiker.Id == default(int)) {
                // New entity
                context.Gebruikers.Add(gebruiker);
            } else {
                // Existing entity
                context.Entry(gebruiker).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var gebruiker = context.Gebruikers.Find(id);
            context.Gebruikers.Remove(gebruiker);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }
    }

    public interface IGebruikerRepository : IDisposable
    {
        IQueryable<Gebruiker> All { get; }
        IQueryable<Gebruiker> AllSorted(string jtSortOrder);
        IQueryable<Gebruiker> AllIncluding(params Expression<Func<Gebruiker, object>>[] includeProperties);
        Gebruiker Find(int id);
        void InsertOrUpdate(Gebruiker gebruiker);
        void Delete(int id);
        void Save();
    }
}