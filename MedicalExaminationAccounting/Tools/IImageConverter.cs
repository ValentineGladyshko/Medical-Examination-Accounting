
namespace MedicalExaminationAccounting.Tools
{
    public interface IImageConverter
    {
        byte[] ConvertImage(byte[] bytes);
    }
}