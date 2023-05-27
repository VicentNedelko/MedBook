using DTO;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using MedBook.Models;
using MedBook.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using PDFConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Managers.ResearchesManager
{
    public class ResearchManager
    {
        private readonly MedBookDbContext _medBookDbContext;
        public ResearchManager(MedBookDbContext medBookDbContext)
        {
            _medBookDbContext = medBookDbContext;
        }
        public bool IsResearchDuplicated(Research research)
        {
            var allResearchesForUser = GetResearchesEager(research.PatientId);
            if (research.Num != null && allResearchesForUser.Select(x => x.Num).Contains(research.Num))
            {
                return true;
            }
            foreach (var r in allResearchesForUser)
            {
                if (research.Equals(r))
                {
                    return true;
                }
                continue;
            }
            return false;
        }

        public async Task<ResearchVM> GetResearchDataAsync(string filePath, string patientId)
        {
            var strategies = new List<ITextExtractionStrategy>
                {
                    new SimpleTextExtractionStrategy(),
                    new LocationTextExtractionStrategy()
                };

            var actualSamplesByStrategy = new Dictionary<string, SampleDTO[]>();
            var textByStrategy = new Dictionary<string, string[]>();
            var documentByStrategy = new Dictionary<string, string>();

            foreach (var strategy in strategies)
            {
                string rawText = PdfGetter.PdfToStringConvert(filePath, strategy);
                var text = rawText.Split(new char[] { '\n' });

                string documentAsString = PdfGetter.GetDataFromDocument(rawText);

                var samplesInResearch = PdfGetter
                    .GetActualSampleNames(documentAsString,
                    await _medBookDbContext.SampleIndicators
                    .Select(si => new SampleDTO
                    {
                        Id = si.Id,
                        Name = si.Name,
                        Unit = si.Unit,
                        ReferenceMax = si.ReferenceMax,
                        ReferenceMin = si.ReferenceMin,
                        BearingIndicatorId = si.BearingIndicatorId,
                    })
                    .AsNoTracking()
                    .ToArrayAsync());
                actualSamplesByStrategy.Add(strategy.ToString(), samplesInResearch);
                textByStrategy.Add(strategy.ToString(), text);
                documentByStrategy.Add(strategy.ToString(), documentAsString);
            }

            var winIndex = actualSamplesByStrategy[strategies[0].ToString()].Length
                > actualSamplesByStrategy[strategies[1].ToString()].Length ? 0 : 1;

            System.IO.File.Delete(filePath);

            ResearchVM researchVM = new()
            {
                Laboratory = PdfGetter.GetLaboratoryName(textByStrategy[strategies[winIndex].ToString()]),
                ResearchDate = PdfGetter.GetDateOfResearch(textByStrategy[strategies[winIndex].ToString()]),
                PatientId = patientId,
                Items = new List<ResearchVM.Item>(),
            };

            string pidString = String.Empty;
            string researchPID = String.Empty;
            if (researchVM.Laboratory == PDFConverter.Constants.LaboratoryName.INVITRO)
            {
                pidString = textByStrategy[strategies[winIndex].ToString()].Where(t => t.Contains(PDFConverter.Constants.ResearchPID.RPID_INVITRO))
                                    .FirstOrDefault();
                researchPID = PdfGetter.GetResearchPIDInvitro(pidString);
            }
            else if (researchVM.Laboratory == PDFConverter.Constants.LaboratoryName.SYNEVO)
            {
                researchPID = PdfGetter.GetResearchPIDSynevo(textByStrategy[strategies[winIndex].ToString()]);
            }
            else if (researchVM.Laboratory == PDFConverter.Constants.LaboratoryName.SYNLAB)
            {
                researchPID = PdfGetter.GetResearchPIDSynlab(textByStrategy[strategies[winIndex].ToString()]);
            }
            researchVM.Num = String.IsNullOrEmpty(researchPID) ? "Не определено" : researchPID;

            foreach (var exactIndicator in actualSamplesByStrategy[strategies[winIndex].ToString()])
            {
                var bearInd = await _medBookDbContext.BearingIndicators
                    .FindAsync(exactIndicator.BearingIndicatorId);
                researchVM.Items.Add(new ResearchVM.Item
                {
                    IndicatorName = bearInd.Name,
                    IndicatorValue = PdfGetter.GetParameterValue(documentByStrategy[strategies[winIndex].ToString()], exactIndicator, Convert.ToInt32(bearInd.Type)),
                    IndicatorUnit = bearInd.Unit,
                    IndicatorType = Convert.ToInt32(bearInd.Type),
                    BearingIndicatorId = bearInd.Id,
                });
            }

            return researchVM;
        }

        private List<Research> GetResearchesEager(string patientId)
        {
            var researches = _medBookDbContext.Researches
                                              .Where(r => r.PatientId == patientId)
                                              .ToList();

            foreach (var r in researches)
            {
                r.Indicators = new List<Indicator>();
                r.Indicators = _medBookDbContext.Indicators
                                                .Where(ind => ind.ResearchId == r.Id)
                                                .OrderBy(ind => ind.Name)
                                                .ToList();
            }

            return researches;
        }
    }
}
