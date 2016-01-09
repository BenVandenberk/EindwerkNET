using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class DepartementKlantRepository : IDepartementKlantRepository
    {
        private UurFacContext context;

        public DepartementKlantRepository()
        {
            context = new UurFacContext();
        }

        public DepartementKlantRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<DepartementKlant> All
        {
            get { return context.DepartementKlants; }
        }

        public IQueryable<DepartementKlant> AllIncluding(params Expression<Func<DepartementKlant, object>>[] includeProperties)
        {
            IQueryable<DepartementKlant> query = context.DepartementKlants;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DepartementKlant Find(int id)
        {
            return context.DepartementKlants.Find(id);
        }

        public void InsertOrUpdate(DepartementKlant departementklant)
        {
            if (departementklant.Id == default(int)) {
                // New entity
                context.DepartementKlants.Add(departementklant);
            } else {
                // Existing entity
                context.Entry(departementklant).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var departementklant = context.DepartementKlants.Find(id);
            context.DepartementKlants.Remove(departementklant);
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

    public interface IDepartementKlantRepository : IDisposable
    {
        IQueryable<DepartementKlant> All { get; }
        IQueryable<DepartementKlant> AllIncluding(params Expression<Func<DepartementKlant, object>>[] includeProperties);
        DepartementKlant Find(int id);
        void InsertOrUpdate(DepartementKlant departementklant);
        void Delete(int id);
        void Save();
    }
}