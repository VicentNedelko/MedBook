using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public static string PdfToStringConvert(string filePath)
        {
            string pageContent = String.Empty;
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filePath));
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
            }
            pdfDoc.Close();
            return pageContent;
        }

        public static string[] GetDesiredParameters(string[] inputArray, string[] bearingArray)
        {
            List<string> result = new List<string>();
            foreach(var str in bearingArray)
            {
                result.Add(inputArray.Where(
                    s => s.Contains(str, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault());
            }
            return result.ToArray();
        }
    }
}
