using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Interfaces;

namespace MedicalExaminationAccounting.Model.Repositories
{
    public class RegionRepository : IRepository<Region>
    {
        private DataContext db;

        public RegionRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Region> GetAll()
        {
            return db.Regions;
        }

        public Region Get(int id)
        {
            return db.Regions.Find(id);
        }

        public IEnumerable<Region> Find(Func<Region, bool> predicate)
        {
            return db.Regions.Where(predicate).ToList();
        }

        public void Create(Region item)
        {
            db.Regions.Add(item);
        }

        public void Update(Region item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Regions.Find(id);
            if (item != null)
                db.Regions.Remove(item);
        }
    }
}