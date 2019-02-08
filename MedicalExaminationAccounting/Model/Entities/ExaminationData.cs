using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Media.Imaging;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class ExaminationData
    {
        public int Id { get; set; }
        [Required] public byte[] Data { get; set; }

        [Required] public int ExaminationId { get; set; }
        public virtual Examination Examination { get; set; }

        public BitmapImage SmallImage
        {
            get
            {                        
                BitmapImage image = new BitmapImage();
                using (var stream = new MemoryStream(Data))
                {
                    image.BeginInit();
                    image.DecodePixelHeight = 100;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    image.Freeze();
                }

                return image;
            }
        }
        public DateTime? DeletedDate { get; set; }
    }
}
