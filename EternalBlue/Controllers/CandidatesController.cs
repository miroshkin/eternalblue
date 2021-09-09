﻿using AutoMapper;
using EternalBlue.Data;
using EternalBlue.Ifs;
using EternalBlue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EternalBlue.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly ILogger<CandidatesController> _logger;
        private IIfsDataProvider _dataProvider;
        private IFSContext _context;
        private IMapper _mapper;
        private IEncryptor _encryptor;

        public CandidatesController(ILogger<CandidatesController> logger, IIfsDataProvider dataProvider, IFSContext context, IMapper mapper, IEncryptor encryptor)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            _context = context;
            _mapper = mapper;
            _encryptor = encryptor;
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
            try
            {
                var candidate = JsonConvert.DeserializeObject<Candidate>(_encryptor.Decrypt(candidateInfo));
                var processedCandidate = _mapper.Map<ProcessedCandidate>(candidate);
                processedCandidate.Approved = approved;
                _context.ProcessedCandidates.Add(processedCandidate);
                _context.SaveChanges();
                var message = $"Candidate {candidate.FullName} has been successfully " +
                              (approved ? "approved" : "rejected");
                TempData[TempDataType.SuccessMessage] = message;
                
                _logger.LogInformation(message);
            }
            catch (Exception e)
            {
                TempData[TempDataType.ErrorMessage] = e.Message;
                _logger.LogError(e.Message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Confirm(string status, string candidateInfo, string fullName, int? selectedExperience, string selectedTechnology)
        {
            TempData[TempDataType.SelectedTechnology] = selectedTechnology;
            TempData[TempDataType.SelectedExperience] = selectedExperience;
            return View("Confirm", new ConfirmationPageViewModel(){ Status = status, CandidateInfo = candidateInfo, FullName = fullName});
        }

        public async Task<IActionResult> Index()
        {
            if (TempData["SelectedTechnology"] != null & TempData["SelectedExperience"] != null)
            {
                var model = new CandidatesPageViewModel();
                model.SelectedTechnology = TempData["SelectedTechnology"].ToString();
                model.SelectedExperience = Convert.ToInt32(TempData["SelectedExperience"]);

                var candidates = await _dataProvider.GetItems<Candidate>(IFSHelper.GetResourceName(typeof(Candidate)), new CancellationToken());
                var technologies = await _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken());
                var processedCandidates = await _context.ProcessedCandidates.AsNoTracking().ToListAsync();

                var filteredCandidates = candidates.Where(GetFilter(model.SelectedTechnology, model.SelectedExperience))
                    .Where(c => !processedCandidates.Exists(p => p.Id == c.CandidateId)).ToList();

                LoadTechnologies(model, technologies);
                LoadYearsOfExperience(model);

                SelectTechnology(model);
                SelectExperience(model);

                FillSkillNames(filteredCandidates, technologies);
                model.Candidates = filteredCandidates;

                return View(model);
            }
            else
            {
                var model = new CandidatesPageViewModel();
                var technologies = await _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken());
                LoadTechnologies(model, technologies);
                LoadYearsOfExperience(model);

                return View(model);
            }
        }

        private void SelectExperience(CandidatesPageViewModel model)
        {
            foreach (var item in model.YearsOfExperience)
            {
                if (item.Value == model.SelectedExperience.ToString())
                {
                    item.Selected = true;
                    break;
                }
            }
        }

        private void SelectTechnology(CandidatesPageViewModel model)
        {
            foreach (var item in model.Technologies)
            {
                if (item.Value == model.SelectedTechnology)
                {
                    item.Selected = true;
                    break;
                }
            }
        }

        private void LoadYearsOfExperience(CandidatesPageViewModel model)
        {
            model.YearsOfExperience = new List<SelectListItem>();
            model.YearsOfExperience.Add(new SelectListItem("Any", "0"));
            model.YearsOfExperience.AddRange(Enumerable.Range(1, IFSHelper.GetFortranAge()).Select(c => new SelectListItem(c.ToString(),c.ToString())));
        }

        private static void LoadTechnologies(CandidatesPageViewModel model, ICollection<Technology> technologies)
        {
            model.Technologies = new List<SelectListItem>();
            model.Technologies.Add(new SelectListItem("Any", "Any"));
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
        public async Task<IActionResult> Index(CandidatesPageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var candidates = await _dataProvider.GetItems<Candidate>(IFSHelper.GetResourceName(typeof(Candidate)), new CancellationToken());
                var technologies = await _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken());
                var processedCandidates = await _context.ProcessedCandidates.AsNoTracking().ToListAsync();

                var filteredCandidates = candidates.Where(GetFilter(model.SelectedTechnology, model.SelectedExperience))
                    .Where(c => !processedCandidates.Exists(p => p.Id == c.CandidateId)).ToList();

                FillSkillNames(filteredCandidates, technologies);
                model.Candidates = filteredCandidates;

                LoadTechnologies(model, technologies);
                LoadYearsOfExperience(model);
                return View(model);
            }

            else
            {
                var technologies = await _dataProvider.GetItems<Technology>(IFSHelper.GetResourceName(typeof(Technology)), new CancellationToken());
                LoadTechnologies(model, technologies);
                LoadYearsOfExperience(model);
                return View(model);
            }
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
        
        private Func<Candidate, bool> GetFilter(string technology, int? yearsOfExperience)
        {
            if (technology == "Any")
            {
                return c => c.Experience.Any(s => s.YearsOfExperience >= yearsOfExperience);
            }
            return c => c.Experience.Any(s => s.TechnologyId.ToString() == technology && s.YearsOfExperience >= yearsOfExperience);
        }
    }
}
