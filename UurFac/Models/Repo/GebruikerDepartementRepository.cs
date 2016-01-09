using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class GebruikerDepartementRepository : IGebruikerDepartementRepository
    {
        private UurFacContext context;

        public GebruikerDepartementRepository()
        {
            context = new UurFacContext();
        }

        public GebruikerDepartementRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<GebruikerDepartement> All
        {
            get { return context.GebruikerDepartements; }
        }

        public IQueryable<GebruikerDepartement> AllIncluding(params Expression<Func<GebruikerDepartement, object>>[] includeProperties)
        {
            IQueryable<GebruikerDepartement> query = context.GebruikerDepartements;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public GebruikerDepartement Find(int id)
        {
            return context.GebruikerDepartements.Find(id);
        }

        public void InsertOrUpdate(GebruikerDepartement gebruikerdepartement)
        {
            if (gebruikerdepartement.Id == default(int)) {
                // New entity
                context.GebruikerDepartements.Add(gebruikerdepartement);
            } else {
                // Existing entity
                context.Entry(gebruikerdepartement).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var gebruikerdepartement = context.GebruikerDepartements.Find(id);
            context.GebruikerDepartements.Remove(gebruikerdepartement);
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

    public interface IGebruikerDepartementRepository : IDisposable
    {
        IQueryable<GebruikerDepartement> All { get; }
        IQueryable<GebruikerDepartement> AllIncluding(params Expression<Func<GebruikerDepartement, object>>[] includeProperties);
        GebruikerDepartement Find(int id);
        void InsertOrUpdate(GebruikerDepartement gebruikerdepartement);
        void Delete(int id);
        void Save();
    }
}