using System;

namespace EternalBlue.Models
{
    public class ProcessedCandidateSkill
    {
        public Guid ProcessedCandidateSkillId { get; set; }
        public Guid SkillId { get; set; }
        public Skill Skill { get; set; }
        public Guid ProcessedCandidateId { get; set; }
        public ProcessedCandidate ProcessedCandidate { get; set; }
    }
}
