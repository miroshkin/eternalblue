using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EternalBlue.Models
{
    [Table("Skills")]
    public class Skill
    {
        public Guid SkillId { get; set; }

        public Guid TechnologyId { get; set; }
        
        public int YearsOfExperience { get; set; }

        public string TechnologyName { get; set; }

        public ICollection<ProcessedCandidateSkill> ProcessedCandidateSkills { get; set; }
    }
}
