using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Interfaces;

namespace MedicalExaminationAccounting.Model.Repositories
{
    public class ExaminationDataRepository : IRepository<ExaminationData>
    {
        private DataContext db;

        public ExaminationDataRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<ExaminationData> GetAll()
        {
            return db.ExaminationDatas.Include(item => item.Examination);
        }

        public ExaminationData Get(int id)
        {
            return db.ExaminationDatas.Find(id);
        }

        public IEnumerable<ExaminationData> Find(Func<ExaminationData, bool> predicate)
        {
            return db.ExaminationDatas.Include(item => item.Examination).Where(predicate);
        }

        public void Create(ExaminationData item)
        {
            db.ExaminationDatas.Add(item);
        }

        public void Update(ExaminationData item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.ExaminationDatas.Find(id);
            if (item != null)
                db.ExaminationDatas.Remove(item);
        }
    }
}