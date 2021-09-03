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

    //"fullName": "Krystal Connelly",
    //"firstName": "Krystal",
    //"lastName": "Connelly",
    //"gender": 1,
    //"profilePicture": "https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/280.jpg",
    //"email": "Krystal_Connelly@gmail.com",
    //"favoriteMusicGenre": "World",
    //"dad": "James Ziemann",
    //"mom": "Virginia Jones",
    //"canSwim": false,
    //"barcode": "0170443518568",
    //"experience": [
    //{
    //    "technologyId": "3B85BE83-9B4E-4B15-9EB2-68363936CA07",
    //    "yearsOfExperience": 8
    //}
    //]
    }
}
