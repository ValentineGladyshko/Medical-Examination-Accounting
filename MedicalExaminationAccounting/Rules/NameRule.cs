using System;
using System.Globalization;
using System.Windows.Controls;

namespace MedicalExaminationAccounting.Rules
{
    public class NameRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string localValue = String.Empty;
            try
            {
                localValue = (string) value;
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Поле містить недопустимі символи");
            }

            if (localValue == null)
                return new ValidationResult(false, "Поле повинно містити хоча б 3 символи");
            if (localValue.Length < 3)
                return new ValidationResult(false, "Поле повинно містити хоча б 3 символи");
            if(localValue.Length > 200)
                return new ValidationResult(false, "Поле повинно містити менше 200 символів");

            return ValidationResult.ValidResult;
        }
    }
}