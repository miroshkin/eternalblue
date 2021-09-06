using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EternalBlue.Ifs;

namespace EternalBlue.Data
{
    [ResourceName("candidates")]
    public class Candidate
	{
        [JsonPropertyName("candidateId")]
        public Guid CandidateId { get; set; }

        [JsonPropertyName("experience")]
        public List<Skill> Experience { get; set; }

        [JsonPropertyName("fullname")]
        public string FullName { get; set; }

        [JsonPropertyName("profilePicture")]
        public string ProfilePicture { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
