using Newtonsoft.Json;

namespace OktaRoutingAutomation.Models
{
    public class Resource
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("pos")]
        public Position Position { get; set; }
    }
}
