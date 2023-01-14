using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MedicalExaminationAccounting.Tools
{
    public class JpgQualityConverter : IImageConverter
    {
        public byte[] ConvertImage(byte[] bytes)
        {
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
            return newBytes;
        }
    }
}
