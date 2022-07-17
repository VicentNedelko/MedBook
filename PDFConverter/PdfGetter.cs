using DTO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PDFConverter
{
    public class PdfGetter
    {

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

                while (Char.IsDigit(symb) || Char.IsPunctuation(symb))
                {
                    symb = text[startInd++];
                    len++;
                }
                string valueStr = text.Substring(startVAlueIndex, len - 1);
                if (Double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                {
                    return result;
                }
                return -1;
            }
            else
            {
                string paramValueSubstring = text.Substring(startInd, 15);
                if (paramValueSubstring.Contains("не обнар", StringComparison.OrdinalIgnoreCase))
                {
                    return 0;
                }
                return -1;
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
            return result.GroupBy(x => x.BearingIndicatorId).Select(x => x.First()).ToArray();
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
            foreach (var row in text)
            {
                if (row.Contains("СИНЭВО", StringComparison.OrdinalIgnoreCase)
                    || row.Contains("SYNEVO", StringComparison.OrdinalIgnoreCase))
                {
                    return Constants.LaboratoryName.SYNEVO;
                }
                else if (row.Contains("ИНВИТРО", StringComparison.OrdinalIgnoreCase))
                {
                    return Constants.LaboratoryName.INVITRO;
                }
            }
            return "UNKNOWN";
        }

        public static string GetResearchPIDInvitro(string source)
        {
            var words = source.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var index = Array.IndexOf(words, Constants.ResearchPID.RPID_INVITRO) + 1;
            return words[index];
        }

        public static string GetResearchPIDSynevo(string[] source)
        {
            var pidPattern = new Regex(@"(?:^|\D)(\d{7,8})(?!\d)");
            var st = source.Select(s => s);
            var researchPID = String.Empty;
            for (var i = 0; i < source.Length; i++)
            {
                var str = source[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                researchPID = str.FirstOrDefault(s => pidPattern.IsMatch(s));
                if (researchPID != null) { return researchPID; }
            }
            return "Не определено";
        }
    }
}
