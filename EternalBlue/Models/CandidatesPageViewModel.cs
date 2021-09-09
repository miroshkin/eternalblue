using EternalBlue.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EternalBlue.Models
{
    public class CandidatesPageViewModel
    {
        public List<ProcessedCandidate> ProcessedCandidates { get; set; }
        public List<Candidate> Candidates { get; set; }
        public List<SelectListItem> Technologies { get; set; }
        
        [Required(ErrorMessage = "* required field")]
        public string SelectedTechnology { get; set; }

        [Required(ErrorMessage = "* required field")]
        public int? SelectedExperience { get; set; }

        public List<SelectListItem> YearsOfExperience { get; set; }

        public bool ShowApprovedCandidatesOnly { get; set; }
    }
}
