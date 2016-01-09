using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class GebruikerKlantRepository : IGebruikerKlantRepository
    {
        private UurFacContext context;

        public GebruikerKlantRepository()
        {
            context = new UurFacContext();
        }

        public GebruikerKlantRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<GebruikerKlant> All
        {
            get { return context.GebruikerKlants; }
        }

        public IQueryable<GebruikerKlant> AllIncluding(params Expression<Func<GebruikerKlant, object>>[] includeProperties)
        {
            IQueryable<GebruikerKlant> query = context.GebruikerKlants;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public GebruikerKlant Find(int id)
        {
            return context.GebruikerKlants.Find(id);
        }

        public void InsertOrUpdate(GebruikerKlant gebruikerklant)
        {
            if (gebruikerklant.Id == default(int)) {
                // New entity
                context.GebruikerKlants.Add(gebruikerklant);
            } else {
                // Existing entity
                context.Entry(gebruikerklant).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var gebruikerklant = context.GebruikerKlants.Find(id);
            context.GebruikerKlants.Remove(gebruikerklant);
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

    public interface IGebruikerKlantRepository : IDisposable
    {
        IQueryable<GebruikerKlant> All { get; }
        IQueryable<GebruikerKlant> AllIncluding(params Expression<Func<GebruikerKlant, object>>[] includeProperties);
        GebruikerKlant Find(int id);
        void InsertOrUpdate(GebruikerKlant gebruikerklant);
        void Delete(int id);
        void Save();
    }
}