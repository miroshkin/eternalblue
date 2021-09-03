using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EternalBlue.Ifs;

namespace EternalBlue.Data
{
    [ResourceName("technologies")]
    public class Technology
    {
        [JsonPropertyName("guid")]
        public string TechnologyId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
