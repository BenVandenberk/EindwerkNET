using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class DepartementRepository : IDepartementRepository
    {
        private UurFacContext context;

        public DepartementRepository()
        {
            context = new UurFacContext();
        }

        public DepartementRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<Departement> All
        {
            get { return context.Departements; }
        }

        public IQueryable<Departement> AllSorted(string jtSortOrder)
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
                case "Code":
                    return asc ? All.OrderBy(t => t.Code) : All.OrderByDescending(t => t.Code);
                case "Omschrijving":
                    return asc ? All.OrderBy(t => t.Omschrijving) : All.OrderByDescending(t => t.Omschrijving);            
                default:
                    return All;
            }
        }

        public IQueryable<Departement> AllIncluding(params Expression<Func<Departement, object>>[] includeProperties)
        {
            IQueryable<Departement> query = context.Departements;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Departement Find(int id)
        {
            return context.Departements.Find(id);
        }

        public void InsertOrUpdate(Departement departement)
        {
            if (departement.Id == default(int)) {
                // New entity
                context.Departements.Add(departement);
            } else {
                // Existing entity
                context.Entry(departement).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var departement = context.Departements.Find(id);
            context.Departements.Remove(departement);
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

    public interface IDepartementRepository : IDisposable
    {
        IQueryable<Departement> All { get; }
        IQueryable<Departement> AllSorted(string jtSortOrder);
        IQueryable<Departement> AllIncluding(params Expression<Func<Departement, object>>[] includeProperties);
        Departement Find(int id);
        void InsertOrUpdate(Departement departement);
        void Delete(int id);
        void Save();
    }
}