using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class KlantRepository : IKlantRepository
    {
        private UurFacContext context;

        public KlantRepository()
        {
            context = new UurFacContext();
        }

        public KlantRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<Klant> All
        {
            get { return context.Klants; }
        }

        public IQueryable<Klant> AllIncluding(params Expression<Func<Klant, object>>[] includeProperties)
        {
            IQueryable<Klant> query = context.Klants;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Klant Find(int id)
        {
            return context.Klants.Find(id);
        }

        public void InsertOrUpdate(Klant klant)
        {
            if (klant.Id == default(int)) {
                // New entity
                context.Klants.Add(klant);
            } else {
                // Existing entity
                context.Entry(klant).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var klant = context.Klants.Find(id);
            context.Klants.Remove(klant);
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

    public interface IKlantRepository : IDisposable
    {
        IQueryable<Klant> All { get; }
        IQueryable<Klant> AllIncluding(params Expression<Func<Klant, object>>[] includeProperties);
        Klant Find(int id);
        void InsertOrUpdate(Klant klant);
        void Delete(int id);
        void Save();
    }
}