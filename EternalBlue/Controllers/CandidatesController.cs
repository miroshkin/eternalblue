using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EternalBlue.Data;
using EternalBlue.Ifs;
using EternalBlue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EternalBlue.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly ILogger<CandidatesController> _logger;
        private IIfsDataProvider _dataProvider;
        private IFSContext _context;

        public CandidatesController(ILogger<CandidatesController> logger, IIfsDataProvider dataProvider, IFSContext context)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _context = context;
        }

        public IActionResult Reject(string candidateId)
        {

            var candidate =
                ((List<Candidate>)TempData["candidates"]).First(c => c.CandidateId == Guid.Parse(candidateId));
            
            _context.ProcessedCandidates.Add(new ProcessedCandidate()
            { Id = candidate.CandidateId, Status = CandidateStatus.Rejected });
            _context.SaveChanges();

            return View("Index");
        }

        public IActionResult Approve(string candidateId, string candidateInfo)
        {
            var candidate = JsonConvert.DeserializeObject<Candidate>(candidateInfo);
            
            _context.ProcessedCandidates.Add(new ProcessedCandidate()
                { Id = candidate.CandidateId, Status = CandidateStatus.Approved });
            _context.SaveChanges();

            return View("Index");
        }

        public IActionResult Confirm(string candidateId, string status, string candidateInfo)
        {
            return View("Confirm", new ConfirmationPageViewModel(){Status = status, CandidateId = candidateId, CandidateInfo = candidateInfo});
        }


        public async Task<IActionResult> Index()
        {
            var model = new CandidatesPageViewModel();

            var candidates = await _dataProvider.GetItems<Candidate>(IFSHelper.GetResourceName(typeof(Candidate)), new CancellationToken());
            var technologies = await _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken());
            var processedCandidates = await _context.ProcessedCandidates.ToListAsync();

            model.Candidates = candidates.Where(c => processedCandidates.TrueForAll(pc => c.CandidateId != pc.Id)).ToList();
            FillSkillNames(model.Candidates, technologies);

            model.Technologies = technologies.Select(t => new SelectListItem(t.Name, t.TechnologyId)).OrderBy(t => t.Text).ToList();
            model.YearsOfExperience = 0;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string technology, int yearsOfExperience)
        {
            var model = new CandidatesPageViewModel();

            var candidates = await _dataProvider.GetItems<Candidate>(IFSHelper.GetResourceName(typeof(Candidate)), new CancellationToken());
            var technologies = await _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken());
            var processedCandidates = await _context.ProcessedCandidates.ToListAsync();

            var filtersAll = new List<Func<Candidate, bool>>();
            filtersAll.Add(CreateFilter((technology, yearsOfExperience)));

            var resultFilter = GetResultFilter(filtersAll);

            var searchResult = new List<Candidate>();
            var filteredCandidates = candidates.Where(resultFilter).ToList();
            FillSkillNames(filteredCandidates, technologies);
            model.Candidates = filteredCandidates;
            model.Technologies = technologies.Select(t => new SelectListItem(t.Name, t.TechnologyId)).OrderBy(t => t.Text).ToList();
            model.YearsOfExperience = 0;

            return View(model);
        }

        private void FillSkillNames(ICollection<Candidate> candidates, ICollection<Technology> technologies)
        {
            foreach (var candidate in candidates)
            {
                foreach (var technology in technologies)
                {
                    foreach (var skill in candidate.Experience)
                    {
                        if (skill.TechnologyId == technology.TechnologyId)
                        {
                            skill.TechnologyName = technology.Name;
                        }
                    }
                }

            }
        }

        private static Func<Candidate, bool> GetResultFilter(List<Func<Candidate, bool>> filtersAll)
        {
            Func<Candidate, bool> resultFilter = null;
            for (int i = 0; i < filtersAll.Count; i++)
            {
                if (i == 0)
                {
                    resultFilter = filtersAll.ElementAt(0);
                    continue;
                }

                resultFilter = GetResultFilter(resultFilter, filtersAll[i]);
            }

            return resultFilter;
        }

        private static Func<Candidate, bool> GetResultFilter(Func<Candidate, bool> filter, Func<Candidate, bool> nextFilter)
        {
            return candidate => filter(candidate) && nextFilter(candidate);
        }

        private Func<Candidate, bool> CreateFilter((string, int) filter1)
        {
            return c => c.Experience.Any(s => s.TechnologyId == filter1.Item1 && s.YearsOfExperience >= filter1.Item2);
        }
    }
}
