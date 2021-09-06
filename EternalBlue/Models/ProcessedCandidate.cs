using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EternalBlue.Models
{
    public class ProcessedCandidate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public bool Approved { get; set; }

        public ICollection<ProcessedCandidateSkill> CandidateSkills { get; set; }

        [MaxLength(200)]
        public string FullName { get; set; }

        public string ProfilePicture { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

    }
}
