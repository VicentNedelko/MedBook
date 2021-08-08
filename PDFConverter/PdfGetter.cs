using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PDFConverter
{
    public class PdfGetter
    {
        public static (string name, double value) GetParameterValue(string param, string[] bearingArray)
        {
            var result = (name: String.Empty, value: 0.0);
            List<string> indicatorNames = new List<string>();
            string[] elementsOfString = param.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string str in bearingArray)
            {
                if (param.Contains(str))
                {
                    indicatorNames.Add(str);
                }
            }
            result.name = indicatorNames.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);

            foreach (var str in elementsOfString)
            {
                if (Double.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double val))
                {
                    result.value = val;
                    break;
                }
            };
            return result;
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

        public static DateTime GetResearchDate(string[] model)
        {
            DateTime resultDt = new DateTime();
            var listWithDate = model.ToList();
            var itemsToRemove = model.Where(s => s.Contains("рождения", StringComparison.OrdinalIgnoreCase));

            string[] arrayWithDate = listWithDate.Except(itemsToRemove).FirstOrDefault()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            foreach(var str in arrayWithDate)
            {
                if (DateTime.TryParseExact(str, new string[] { "dd.MM.yyyy", "dd/MM/yyyy" },
                    null, DateTimeStyles.None, out resultDt))
                {
                    return resultDt;
                }

            }
            return resultDt;
        }

        public static string GetLaboratoryName(string[] text)
        {
            foreach(var row in text)
            {
                if(row.Contains("СИНЭВО", StringComparison.OrdinalIgnoreCase)
                    || row.Contains("SYNEVO", StringComparison.OrdinalIgnoreCase))
                {
                    return "ООО СИНЭВО";
                }
                else if(row.Contains("ИНВИТРО", StringComparison.OrdinalIgnoreCase))
                {
                    return "ИООО \"Независимая лаборатория ИНВИТРО\"";
                }
            }
            return "UNKNOWN";
        }
    }
}
