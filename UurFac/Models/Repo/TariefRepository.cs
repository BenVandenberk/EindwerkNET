using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class TariefRepository : ITariefRepository
    {
        private UurFacContext context;

        public TariefRepository()
        {
            context = new UurFacContext();
        }

        public TariefRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<Tarief> All
        {
            get { return context.Tariefs; }
        }

        public IQueryable<Tarief> AllSorted(string jtSortOrder)
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
                case "TypeWerk":
                    return asc ? All.OrderBy(t => t.TypeWerk) : All.OrderByDescending(t => t.TypeWerk);
                case "GeldigVanaf":
                    return asc ? All.OrderBy(t => t.GeldigVanaf) : All.OrderByDescending(t => t.GeldigVanaf);
                case "TariefUurWaarde":
                    return asc ? All.OrderBy(t => t.TariefUurWaarde) : All.OrderByDescending(t => t.TariefUurWaarde);
                default:
                    return All;
            }
        }

        public IQueryable<Tarief> AllIncluding(params Expression<Func<Tarief, object>>[] includeProperties)
        {
            IQueryable<Tarief> query = context.Tariefs;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Tarief Find(int id)
        {
            return context.Tariefs.Find(id);
        }

        public void InsertOrUpdate(Tarief tarief)
        {
            if (tarief.Id == default(int)) {
                // New entity
                context.Tariefs.Add(tarief);
            } else {
                // Existing entity
                context.Entry(tarief).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var tarief = context.Tariefs.Find(id);
            context.Tariefs.Remove(tarief);
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

    public interface ITariefRepository : IDisposable
    {
        IQueryable<Tarief> All { get; }
        IQueryable<Tarief> AllSorted(string jtSortOrder);
        IQueryable<Tarief> AllIncluding(params Expression<Func<Tarief, object>>[] includeProperties);
        Tarief Find(int id);
        void InsertOrUpdate(Tarief tarief);
        void Delete(int id);
        void Save();
    }
}