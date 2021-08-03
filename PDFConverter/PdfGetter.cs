using System;

namespace PDFConverter
{
    public class PdfGetter
    {
        public static double GetParameterValue(string param)
        {
            double value = 0.0;
            string[] elementsOfString = param.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var str in elementsOfString)
            {
                if (Double.TryParse(str, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out value))
                {
                    Console.WriteLine(value);
                    break;
                }
            };
            return value;
        }
    }
}
