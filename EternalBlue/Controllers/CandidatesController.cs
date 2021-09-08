using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EternalBlue.Data;
using EternalBlue.Ifs;
using EternalBlue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Skill = EternalBlue.Models.Skill;

namespace EternalBlue.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly ILogger<CandidatesController> _logger;
        private IIfsDataProvider _dataProvider;
        private IFSContext _context;
        private IMapper _mapper;

        public CandidatesController(ILogger<CandidatesController> logger, IIfsDataProvider dataProvider, IFSContext context, IMapper mapper)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Reject(string candidateInfo)
        {
            return ProcessCandidate(candidateInfo, false);
        }

        public IActionResult Approve(string candidateInfo)
        {
            return ProcessCandidate(candidateInfo, true);
        }

        private IActionResult ProcessCandidate(string candidateInfo, bool approved)
        {
            var candidate = JsonConvert.DeserializeObject<Candidate>(candidateInfo);
            var processedCandidate = _mapper.Map<ProcessedCandidate>(candidate);
            processedCandidate.Approved = approved;
            _context.ProcessedCandidates.Add(processedCandidate);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Confirm(string candidateId, string status, string candidateInfo, string fullName)
        {
            return View("Confirm", new ConfirmationPageViewModel(){ Status = status, CandidateId = candidateId, CandidateInfo = candidateInfo, FullName = fullName});
        }

        public async Task<IActionResult> Index()
        {
            var model = new CandidatesPageViewModel();

            var candidates = await _dataProvider.GetItems<Candidate>(IFSHelper.GetResourceName(typeof(Candidate)), new CancellationToken());
            var technologies = await _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken());
            var processedCandidates = await _context.ProcessedCandidates.AsNoTracking().ToListAsync();

            model.Candidates = candidates.Where(c => !processedCandidates.Exists(p => p.Id == c.CandidateId)).ToList();


            FillSkillNames(model.Candidates, technologies);

            LoadTechnologies(model, technologies);
            LoadYearsOfExperience(model);

            return View(model);
        }

        private void LoadYearsOfExperience(CandidatesPageViewModel model)
        {
            model.YearsOfExperience = new List<SelectListItem>();
            model.YearsOfExperience.Add(new SelectListItem("Any", "0", true));
            model.YearsOfExperience.AddRange(Enumerable.Range(1, IFSHelper.GetFortranAge()).Select(c => new SelectListItem(c.ToString(),c.ToString())));
        }

        private static void LoadTechnologies(CandidatesPageViewModel model, ICollection<Technology> technologies)
        {
            model.Technologies = new List<SelectListItem>();
            model.Technologies.Add(new SelectListItem("Any", "Any", true));
            model.Technologies.AddRange(technologies.Select(t => new SelectListItem(t.Name, t.TechnologyId.ToString()))
                .OrderBy(t => t.Text).ToList());
        }

        public async Task<IActionResult> Approved()
        {
            var model = new CandidatesPageViewModel(){ShowApprovedCandidatesOnly = true};

            var processedCandidates = await _context.ProcessedCandidates
                    .Include(c => c.ProcessedCandidateSkills)
                    .ThenInclude(s => s.Skill)
                    .Include(c => c.ProcessedCandidateSkills)
                    .ThenInclude(c => c.ProcessedCandidate)
                    .Where(c => c.Approved)
                    .ToListAsync();

            model.Candidates = _mapper.Map<List<Candidate>>(processedCandidates);

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string technology, int yearsOfExperience)
        {
            var model = new CandidatesPageViewModel();

            var candidates = await _dataProvider.GetItems<Candidate>(IFSHelper.GetResourceName(typeof(Candidate)), new CancellationToken());
            var technologies = await _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken());
            var processedCandidates = await _context.ProcessedCandidates.AsNoTracking().ToListAsync();


            var filteredCandidates = candidates.Where(GetFilter(technology, yearsOfExperience))
                .Where(c => !processedCandidates.Exists(p => p.Id == c.CandidateId)).ToList();

            FillSkillNames(filteredCandidates, technologies);
            model.Candidates = filteredCandidates;

            LoadTechnologies(model, technologies);
            LoadYearsOfExperience(model);

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

        private Func<Candidate, bool> GetFilter(string technology, int yearsOfExperience)
        {
            if (technology == "Any")
            {
                return c => c.Experience.Any(s => s.YearsOfExperience >= yearsOfExperience);
            }
            return c => c.Experience.Any(s => s.TechnologyId.ToString() == technology && s.YearsOfExperience >= yearsOfExperience);
        }
    }
}
