using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EternalBlue.Data
{
    public class Skill
    {
        [JsonPropertyName("technologyId")]
        public string TechnologyId { get; set; }

        [JsonPropertyName("yearsOfExperience")]
        public int YearsOfExperience { get; set; }

        public string TechnologyName { get; set; }
    }
}
