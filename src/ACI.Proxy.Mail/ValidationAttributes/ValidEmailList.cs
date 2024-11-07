using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ACI.Proxy.Mail.ValidationAttributes
{
    public class ValidEmailList : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentUICulture, ErrorMessageString, MinLength, MaxLength);
        }

        public override bool IsValid(object? value)
        {
            var list = value as IList;
            if (list != null)
            {
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if ((item == null) || !(item is string) || (((string)item).Length < MinLength) || (((string)item).Length > MaxLength))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public int MaxLength { get; set; }

        public int MinLength { get; set; }
    }
}
