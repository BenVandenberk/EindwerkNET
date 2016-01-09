using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class FactuurRepository : IFactuurRepository
    {
        private UurFacContext context;

        public FactuurRepository()
        {
            context = new UurFacContext();
        }

        public FactuurRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<Factuur> All
        {
            get { return context.Factuurs; }
        }

        public IQueryable<Factuur> AllIncluding(params Expression<Func<Factuur, object>>[] includeProperties)
        {
            IQueryable<Factuur> query = context.Factuurs;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Factuur Find(int id)
        {
            return context.Factuurs.Find(id);
        }

        public void InsertOrUpdate(Factuur factuur)
        {
            if (factuur.Id == default(int)) {
                // New entity
                context.Factuurs.Add(factuur);
            } else {
                // Existing entity
                context.Entry(factuur).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var factuur = context.Factuurs.Find(id);
            context.Factuurs.Remove(factuur);
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

    public interface IFactuurRepository : IDisposable
    {
        IQueryable<Factuur> All { get; }
        IQueryable<Factuur> AllIncluding(params Expression<Func<Factuur, object>>[] includeProperties);
        Factuur Find(int id);
        void InsertOrUpdate(Factuur factuur);
        void Delete(int id);
        void Save();
    }
}