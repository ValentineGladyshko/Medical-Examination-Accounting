using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Interfaces;

namespace MedicalExaminationAccounting.Model.Repositories
{
    public class StreetRepository : IRepository<Street>
    {
        private DataContext db;

        public StreetRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Street> GetAll()
        {
            return db.Streets.Include(item => item.Settlement.Region);
        }

        public Street Get(int id)
        {
            return db.Streets.Find(id);
        }

        public IEnumerable<Street> Find(Func<Street, bool> predicate)
        {
            return db.Streets.Include(item => item.Settlement.Region).ToList().Where(predicate);
        }

        public void Create(Street item)
        {
            db.Streets.Add(item);
        }

        public void Update(Street item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Streets.Find(id);
            if (item != null)
                db.Streets.Remove(item);
        }
    }
}