using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Interfaces;

namespace MedicalExaminationAccounting.Model.Repositories
{
    public class ExaminationRepository : IRepository<Examination>
    {
        private DataContext db;

        public ExaminationRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Examination> GetAll()
        {
            return db.Examinations.Include(item => item.Patient)
                .Include(item => item.Doctor)
                .Include(item => item.ExaminationDatas);
        }

        public Examination Get(int id)
        {
            return db.Examinations.Find(id);
        }

        public IEnumerable<Examination> Find(Func<Examination, bool> predicate)
        {
            return db.Examinations.Include(item => item.Patient)
                .Include(item => item.Doctor)
                .Include(item => item.ExaminationDatas).Where(predicate);
        }

        public void Create(Examination item)
        {
            db.Examinations.Add(item);
        }

        public void Update(Examination item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Examinations.Find(id);
            if (item != null)
                db.Examinations.Remove(item);
        }
    }
}