using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        //public static String RemoveDiacritics(string s)
        //{
        //    string normalizedString = s.Normalize(NormalizationForm.FormD);
        //    StringBuilder stringBuilder = new StringBuilder();
        //    for (int i = 0; i < normalizedString.Length; i++)
        //    {
        //        char c = normalizedString[i];
        //        if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        //            stringBuilder.Append(c);
        //    }
        //    return stringBuilder.ToString();
        //}
        public static String RemoveDiacritics(this String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}