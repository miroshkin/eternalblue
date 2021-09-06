using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EternalBlue.Models
{
    public class Skill
    {
        public Guid SkillId { get; set; }

        public Guid TechnologyId { get; set; }
        
        public int YearsOfExperience { get; set; }

        public string TechnologyName { get; set; }

        public ICollection<ProcessedCandidateSkill> ProcessedCandidateSkills { get; set; }
    }
}
