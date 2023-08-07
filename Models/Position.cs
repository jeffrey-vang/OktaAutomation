using Newtonsoft.Json;

namespace OktaRoutingAutomation.Models
{
    public class Position
    {
        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("line")]
        public int LineNumber { get; set; }
    }
}
