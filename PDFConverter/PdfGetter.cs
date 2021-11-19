using DTO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PDFConverter
{
    public class PdfGetter
    {
        //public static (string name, double value) GetParameterValue(string param, string[] bearingArray)
        //{
        //    var result = (name: String.Empty, value: 0.0);
        //    List<string> indicatorNames = new List<string>();
        //    string[] elementsOfString = param.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach(string str in bearingArray)
        //    {
        //        if (param.Contains(str))
        //        {
        //            indicatorNames.Add(str);
        //        }
        //    }
        //    result.name = indicatorNames.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);

        //    foreach (var str in elementsOfString)
        //    {
        //        if (Double.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double val))
        //        {
        //            result.value = val;
        //            break;
        //        }
        //    };
        //    return result;
        //}

        public static double GetParameterValue(string text, SampleDTO param, int type)
        {
            int startInd = param.StartIndex + param.Name.Length;
            var fromStart = text.IndexOf(param.Name);
            var res = text.Substring(fromStart, param.Name.Length);
            if (type == 0)
            {
                char symb = text[startInd];
                while (!Char.IsDigit(symb))
                {
                    startInd++;
                    symb = text[startInd];
                }
                int startVAlueIndex = startInd;
                int len = 0;

                while (symb != ' ')
                {
                    symb = text[startInd++];
                    len++;
                }
                string valueStr = text.Substring(startVAlueIndex, len);
                if (Double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                {
                    return result;
                }
                return -1;
            }
            else
            {
                string paramValueSubstring = text.Substring(startInd, 15);
                if(paramValueSubstring.Contains("не обнар", StringComparison.OrdinalIgnoreCase))
                {
                    return 0;
                }
                else if(paramValueSubstring.Contains("обнар", StringComparison.OrdinalIgnoreCase)) // check Research with positive result
                {
                    return -1;
                }
                return -2;
            }

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

        public static SampleDTO[] GetActualSampleNames(string inputString, SampleDTO[] bearingArray)
        {
            List<SampleDTO> result = new List<SampleDTO>();
            foreach (var sample in bearingArray)
            {
                if (inputString.Contains(sample.Name))
                {
                    sample.StartIndex = inputString.IndexOf(sample.Name);
                    result.Add(sample);
                }
            }
            return result.ToArray();
        }

        private static int[] GetEntryIndexes(string inputString, string str)
        {
            int index = 0;
            List<int> result = new List<int>();
            while(index < inputString.LastIndexOf(str))
            {
                int entryPoint = inputString.IndexOf(str, index, StringComparison.OrdinalIgnoreCase);
                result.Add(entryPoint);
                index += entryPoint + str.Length;
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

            if (arrayWithDate != null)
            {
                foreach (var str in arrayWithDate)
                {
                    if (DateTime.TryParseExact(str, new string[] { "dd.MM.yyyy", "dd/MM/yyyy" },
                        null, DateTimeStyles.None, out resultDt))
                    {
                        return resultDt;
                    }

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
