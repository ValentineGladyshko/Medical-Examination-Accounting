using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Tools;

namespace MedicalExaminationAccounting.Model.Context
{
    public class DataContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Settlement> Settlements { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<ExaminationType> ExaminationTypes { get; set; }
        public DbSet<ExaminationData> ExaminationDatas { get; set; }

        static DataContext()
        {
            Database.SetInitializer<DataContext>(new StoreDbInitializer());
        }

        private static DataContext dataContext;
        private DataContext(string connectionString) : base(connectionString)
        {
        }

        public static DataContext GetDataContext(string connectionString)
        {
            if (dataContext == null)
            {
                dataContext = new DataContext(connectionString);
            }

            return dataContext;
        }
    }

    public class StoreDbInitializer : DropCreateDatabaseIfModelChanges<DataContext>
    {
        protected override void Seed(DataContext db)
        {
            ExaminationTypeInit(db);
            PatientInit(db);
            ExaminationInit(db);
        }

        private void RegionInit(DataContext db)
        {
            string path = @"C:\Users\Kappi\Source\Repos\Medical-Examination-Accounting\MedicalExaminationAccounting\Strings";
            string[] regionNames = File.ReadAllLines(path + @"\regions.txt");
            foreach (var name in regionNames)
            {
                db.Regions.Add(new Region
                {
                    RegionName = name
                });
            }
            
            db.SaveChanges();
        }

        private void ExaminationTypeInit(DataContext db)
        {
            db.ExaminationTypes.Add(new ExaminationType
            {
                TypeName = "МРТ",
                DeletedDate = null
            });

            db.ExaminationTypes.Add(new ExaminationType
            {
                TypeName = "ЕКГ",
                DeletedDate = null
            });

            db.SaveChanges();
        }

        private void PatientInit(DataContext db)
        {
            string path = @"C:\Users\Kappi\Source\Repos\Medical-Examination-Accounting\MedicalExaminationAccounting\Strings";
            string[] womanFirstNames = File.ReadAllLines(path + @"\womanfirstnames.txt");
            string[] womanMiddleNames = File.ReadAllLines(path + @"\womanmiddlenames.txt");
            string[] womanLastNames = File.ReadAllLines(path + @"\womanlastnames.txt");
            string[] manFirstNames = File.ReadAllLines(path + @"\manfirstnames.txt");
            string[] manMiddleNames = File.ReadAllLines(path + @"\manmiddlenames.txt");
            string[] manLastNames = File.ReadAllLines(path + @"\manlastnames.txt");

            db.Regions.Add(new Region
            {
                RegionName = "Київська",
                DeletedDate = null
            });

            db.SaveChanges();

            db.Settlements.Add(new Settlement
            {
                SettlementName = "Київ",
                RegionId = db.Regions.First().Id,
                DeletedDate = null
            });

            db.SaveChanges();

            db.Streets.Add(new Street
            {
                StreetName = "Політехнічний провулок",
                SettlementId = db.Settlements.First().Id,
                DeletedDate = null
            });

            db.SaveChanges();

            Random rand = new Random();
            var list = new List<Patient>();
            int id = db.Streets.First().Id;
            for (int i = 0; i < 1000; i++)
            {
                var patient = new Patient();
                if (rand.Next(0, 2) == 0)
                {
                    patient.FirstName = womanFirstNames[rand.Next(0, womanFirstNames.Length)];
                    patient.MiddleName = womanMiddleNames[rand.Next(0, womanMiddleNames.Length)];
                    patient.LastName = womanLastNames[rand.Next(0, womanLastNames.Length)];
                }
                else
                {
                    patient.FirstName = manFirstNames[rand.Next(0, manFirstNames.Length)];
                    patient.MiddleName = manMiddleNames[rand.Next(0, manMiddleNames.Length)];
                    patient.LastName = manLastNames[rand.Next(0, manLastNames.Length)];
                }

                patient.BirthDate = DateTime.Today;
                patient.StreetId = id;
                patient.DeletedDate = null;
                list.Add(patient);
            }

            db.Patients.AddRange(list);
            db.SaveChanges();
        }

        private void ExaminationInit(DataContext db)
        {
            Random rand = new Random();

            byte[] bytes = File.ReadAllBytes("../../Images/forest.jpg");
            PngToJpgConverter converter = new PngToJpgConverter();
            var some = converter.ConvertImage(bytes);
            var stream1 = new MemoryStream(bytes);
            JpegBitmapDecoder decoder = new JpegBitmapDecoder(stream1, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            BitmapFrame frame = decoder.Frames[0];
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = 60;
            encoder.Frames.Add(frame);
            byte[] newBytes;
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                newBytes = stream.GetBuffer();
            }

            byte[] bytes2 = File.ReadAllBytes("../../Images/mountain.jpg");
            var stream2 = new MemoryStream(bytes2);
            var some2 = converter.ConvertImage(bytes2);
            JpegBitmapDecoder decoder2 = new JpegBitmapDecoder(stream2, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            BitmapFrame frame2 = decoder2.Frames[0];
            JpegBitmapEncoder encoder2 = new JpegBitmapEncoder();
            encoder2.QualityLevel = 60;
            encoder2.Frames.Add(frame2);
            byte[] newBytes2;
            using (var stream = new MemoryStream())
            {
                encoder2.Save(stream);
                newBytes2 = stream.GetBuffer();
            }

            var list = db.Patients.ToList();
            foreach (var patient in list)
            {
                if(rand.Next(0,2)==0)
                    continue;
                for (int i = 0; i < 1; i++)
                {
                    ExaminationType type = db.ExaminationTypes.ToList()
                        [rand.Next(0, db.ExaminationTypes.ToList().Count)];

                    Examination examination = new Examination
                    {
                        Diagnosis = "Some Diagnosis",
                        PatientId = patient.Id,
                        ExaminationTypeId = type.Id,
                        ExaminationDate = DateTime.Now,
                        Descripton = "Some Descripton"
                    };

                    Examination examination2 = new Examination
                    {
                        Diagnosis = "Some Diagnosis2",
                        PatientId = patient.Id,
                        ExaminationTypeId = type.Id,
                        ExaminationDate = DateTime.Now,
                        Descripton = "Some Descripton2"
                    };

                    db.Examinations.Add(examination);
                    db.Examinations.Add(examination2);
                    db.SaveChanges();

                    ExaminationData data = new ExaminationData
                    {
                        ExaminationId = examination.Id,
                        Data = newBytes
                    };

                    ExaminationData data2 = new ExaminationData
                    {
                        ExaminationId = examination.Id,
                        Data = newBytes2
                    };

                    db.ExaminationDatas.Add(data);
                    db.ExaminationDatas.Add(data2);
                    db.SaveChanges();
                }
            }
        }
    }
}