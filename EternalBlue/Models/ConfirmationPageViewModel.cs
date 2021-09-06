using EternalBlue.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EternalBlue.Models
{
    public class ConfirmationPageViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
    }
}
