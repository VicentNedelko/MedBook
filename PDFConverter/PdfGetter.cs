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
            _ = text.Substring(fromStart, param.Name.Length);
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

                while (Char.IsDigit(symb) || symb == '.' || symb == ',')
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

        public static string GetDataFromDocument(string rawText)
        {
            string plainText = Regex.Replace(rawText, @"\t|\n|\r", " ");
            Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
            string replacedWithSpaces = regex.Replace(plainText, " ");
            string clearedFromStars = replacedWithSpaces.Replace("*", string.Empty);

            return clearedFromStars;
        }

        public static DateTime GetDateOfResearch(string[] text)
        {
            var researchDateStringArray = text.Where(t => t.Contains(
                "Дата", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            DateTime dateOfResearch = DateTime.Now;
            if (researchDateStringArray.Length != 0)
            {
                dateOfResearch = PdfGetter.GetResearchDate(researchDateStringArray);
            }

            return dateOfResearch;
        }

        public static string PdfToStringConvert(string filePath, ITextExtractionStrategy strategy)
        {
            string pageContent = String.Empty;
            PdfDocument pdfDoc = new(new PdfReader(filePath));
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
            }
            pdfDoc.Close();
            return pageContent;
        }

        public static SampleDTO[] GetActualSampleNames(string inputString, SampleDTO[] bearingArray)
        {
            List<SampleDTO> result = new();
            foreach (var sample in bearingArray)
            {
                if (inputString.Contains(sample.Name))
                {
                    var entrieIndexes = GetAllSampleEntrieIndexes(inputString, sample.Name);
                    foreach (var entrieIndex in entrieIndexes)
                    {
                        var endIndex = entrieIndex + sample.Name.Length + 1;
                        while (Char.IsWhiteSpace(inputString[endIndex]))
                        {
                            endIndex++;
                        }
                        if (Char.IsDigit(inputString[endIndex]))
                        {
                            sample.StartIndex = entrieIndex;
                            result.Add(sample);
                        }
                    }
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
                else if (row.Contains("СИНЛАБ", StringComparison.OrdinalIgnoreCase))
                {
                    return Constants.LaboratoryName.SYNLAB;
                }
                else if (row.Contains("ЛАБОРАТОРИЯ ГЕМОТЕСТ", StringComparison.OrdinalIgnoreCase))
                {
                    return Constants.LaboratoryName.GEMOTEST;
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

        public static string GetResearchPIDSynlab(string[] source)
        {
            var targetString = source.Where(x => x.Contains("PID")).FirstOrDefault();
            if (targetString != null)
            {
                return Regex.Match(targetString, "\\d+").Value;
            }
            return "UNKNOWN";
        }

        private static List<int> GetAllSampleEntrieIndexes(string source, string sampleName)
        {
            if (String.IsNullOrEmpty(sampleName))
                throw new ArgumentException("the string to find may not be empty");
            List<int> indexes = new();
            for (int index = 0; ; index += sampleName.Length)
            {
                index = source.IndexOf(sampleName, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}
