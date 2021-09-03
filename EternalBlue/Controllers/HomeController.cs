using EternalBlue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using EternalBlue.Data;
using EternalBlue.Ifs;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EternalBlue.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IIfsDataProvider _dataProvider;
        private IFSContext _context;

        public HomeController(ILogger<HomeController> logger, IIfsDataProvider dataProvider, IFSContext context)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _context = context;
        }

        public IActionResult Approve(string id)
        {
            _context.ProcessedCandidates.Add(new ProcessedCandidate()
                {Id = Guid.Parse(id), Status = CandidateStatus.Approved});
            _context.SaveChanges();

            return View("Index");
        }

        public IActionResult Reject(string id)
        {
            _context.ProcessedCandidates.Add(new ProcessedCandidate()
                { Id = Guid.Parse(id), Status = CandidateStatus.Rejected });
            _context.SaveChanges();

            return View("Index");
        }

        public IActionResult Index()
        {
            var model = new HomePageViewModel();

            var candidates = _dataProvider.GetItems<Candidate>(IFSHelper.GetResourceName(typeof(Candidate)),new CancellationToken()).ToList();
            var technologies = _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken()).ToList();
            var processedCandidates = _context.ProcessedCandidates.ToList();
            model.Candidates = candidates.Where(c => processedCandidates.TrueForAll(pc => c.CandidateId != pc.Id)).ToList();
            model.Technologies = technologies.Select(t => new SelectListItem(t.Name,t.TechnologyId)).OrderBy(t => t.Text).ToList();
            model.YearsOfExperience = 0;

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(string technology, int yearsOfExperience)
        {
            var model = new HomePageViewModel();

            var candidates = _dataProvider.GetItems<Candidate>(IFSHelper.GetResourceName(typeof(Candidate)), new CancellationToken()).ToList();
            var technologies = _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken()).ToList();
            var processedCandidates = _context.ProcessedCandidates.ToList();

            var filtersAll = new List<Func<Candidate, bool>>();
            filtersAll.Add(CreateFilter((technology, yearsOfExperience)));
            

            var resultFilter = GetResultFilter(filtersAll);

            var searchResult = new List<Candidate>();
            var filteredCandidates = candidates.Where(resultFilter).ToList();
            model.Candidates = filteredCandidates;
            model.Technologies = technologies.Select(t => new SelectListItem(t.Name, t.TechnologyId)).OrderBy(t => t.Text).ToList();
            model.YearsOfExperience = 0;

            return View(model);
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
