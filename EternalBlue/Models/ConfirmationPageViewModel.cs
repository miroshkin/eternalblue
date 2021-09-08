using EternalBlue.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EternalBlue.Models
{
    public class ConfirmationPageViewModel
    {
        public string Status { get; set; }
        public string FullName { get; set; }
        public string CandidateInfo { get; set; }
    }
}
