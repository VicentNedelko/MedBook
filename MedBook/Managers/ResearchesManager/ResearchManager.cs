using MedBook.Models;
using System.Linq;
using DeepEqual;
using DeepEqual.Syntax;
using System.Collections.Generic;

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
            foreach (var r in allResearchesForUser)
            {
                if (research.Equals(r))
                {
                    return true;
                }
                else
                {
                    continue;
                }
            }
            return false;
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
                                                .ToList();
            }

            return researches;
        }
    }
}
