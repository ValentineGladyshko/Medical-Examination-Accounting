using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Interfaces;

namespace MedicalExaminationAccounting.Model.Repositories
{
    public class SettlementRepository : IRepository<Settlement>
    {
        private DataContext db;

        public SettlementRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Settlement> GetAll()
        {
            return db.Settlements.Include(item => item.Region);
        }

        public Settlement Get(int id)
        {
            return db.Settlements.Find(id);
        }

        public IEnumerable<Settlement> Find(Func<Settlement, bool> predicate)
        {
            return db.Settlements.Include(item => item.Region).ToList().Where(predicate);
        }

        public void Create(Settlement item)
        {
            db.Settlements.Add(item);
        }

        public void Update(Settlement item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Settlements.Find(id);
            if (item != null)
                db.Settlements.Remove(item);
        }
    }
}