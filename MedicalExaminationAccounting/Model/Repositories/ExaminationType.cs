using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Interfaces;

namespace MedicalExaminationAccounting.Model.Repositories
{
    public class ExaminationTypeRepository : IRepository<ExaminationType>
    {
        private DataContext db;

        public ExaminationTypeRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<ExaminationType> GetAll()
        {
            return db.ExaminationTypes.Include(item => item.Examinations);
        }

        public ExaminationType Get(int id)
        {
            return db.ExaminationTypes.Find(id);
        }

        public IEnumerable<ExaminationType> Find(Func<ExaminationType, bool> predicate)
        {
            return db.ExaminationTypes.Include(item => item.Examinations).Where(predicate);
        }

        public void Create(ExaminationType item)
        {
            db.ExaminationTypes.Add(item);
        }

        public void Update(ExaminationType item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.ExaminationTypes.Find(id);
            if (item != null)
                db.ExaminationTypes.Remove(item);
        }
    }
}