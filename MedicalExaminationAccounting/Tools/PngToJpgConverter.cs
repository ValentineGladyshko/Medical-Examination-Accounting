using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MedicalExaminationAccounting.Tools
{
    public class PngToJpgConverter : IImageConverter
    {
        public byte[] ConvertImage(byte[] bytes)
        {
            Bitmap d = null;
            var stream1 = new MemoryStream(bytes);
            d = (Bitmap)Bitmap.FromStream(stream1);
            byte[] newBytes;
            using (var stream = new MemoryStream())
            {
                d.Save(stream, ImageFormat.Jpeg);
                newBytes = stream.GetBuffer();
            }
            return newBytes;
        }
    }
}