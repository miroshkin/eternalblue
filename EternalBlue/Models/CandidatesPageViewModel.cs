﻿using EternalBlue.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EternalBlue.Models
{
    public class CandidatesPageViewModel
    {
        public List<ProcessedCandidate> ProcessedCandidates { get; set; }
        public List<Candidate> Candidates { get; set; }
        public List<SelectListItem> Technologies { get; set; }
        public SelectListItem Technology { get; set; }

        public List<SelectListItem> YearsOfExperience { get; set; }

        public bool ShowApprovedCandidatesOnly { get; set; }
    }
}
