using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using UurFac.Models.UurFac;

namespace UurFac.Models.Repo
{ 
    public class UurRegistratieDetailRepository : IUurRegistratieDetailRepository
    {
        private UurFacContext context;

        public UurRegistratieDetailRepository()
        {
            context = new UurFacContext();
        }

        public UurRegistratieDetailRepository(UurFacContext context)
        {
            this.context = context;
        }

        public IQueryable<UurRegistratieDetail> All
        {
            get { return context.UurRegistratieDetails; }
        }

        public IQueryable<UurRegistratieDetail> AllIncluding(params Expression<Func<UurRegistratieDetail, object>>[] includeProperties)
        {
            IQueryable<UurRegistratieDetail> query = context.UurRegistratieDetails;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public UurRegistratieDetail Find(int id)
        {
            return context.UurRegistratieDetails.Find(id);
        }

        public void InsertOrUpdate(UurRegistratieDetail uurregistratiedetail)
        {
            if (uurregistratiedetail.Id == default(int)) {
                // New entity
                context.UurRegistratieDetails.Add(uurregistratiedetail);
            } else {
                // Existing entity
                context.Entry(uurregistratiedetail).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var uurregistratiedetail = context.UurRegistratieDetails.Find(id);
            context.UurRegistratieDetails.Remove(uurregistratiedetail);
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

    public interface IUurRegistratieDetailRepository : IDisposable
    {
        IQueryable<UurRegistratieDetail> All { get; }
        IQueryable<UurRegistratieDetail> AllIncluding(params Expression<Func<UurRegistratieDetail, object>>[] includeProperties);
        UurRegistratieDetail Find(int id);
        void InsertOrUpdate(UurRegistratieDetail uurregistratiedetail);
        void Delete(int id);
        void Save();
    }
}