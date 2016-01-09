using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class UurRegistratieRepository : IUurRegistratieRepository
    {
        private UurFacContext context;

        public UurRegistratieRepository()
        {
            context = new UurFacContext();
        }

        public UurRegistratieRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<UurRegistratie> All
        {
            get { return context.UurRegistraties; }
        }

        public IQueryable<UurRegistratie> AllIncluding(params Expression<Func<UurRegistratie, object>>[] includeProperties)
        {
            IQueryable<UurRegistratie> query = context.UurRegistraties;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public UurRegistratie Find(int id)
        {
            return context.UurRegistraties.Find(id);
        }

        public void InsertOrUpdate(UurRegistratie uurregistratie)
        {
            if (uurregistratie.Id == default(int)) {
                // New entity
                context.UurRegistraties.Add(uurregistratie);
            } else {
                // Existing entity
                context.Entry(uurregistratie).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var uurregistratie = context.UurRegistraties.Find(id);
            context.UurRegistraties.Remove(uurregistratie);
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

    public interface IUurRegistratieRepository : IDisposable
    {
        IQueryable<UurRegistratie> All { get; }
        IQueryable<UurRegistratie> AllIncluding(params Expression<Func<UurRegistratie, object>>[] includeProperties);
        UurRegistratie Find(int id);
        void InsertOrUpdate(UurRegistratie uurregistratie);
        void Delete(int id);
        void Save();
    }
}