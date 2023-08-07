using Newtonsoft.Json;

namespace OktaAutomation.Models
{
    public class Position
    {
        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("line")]
        public int LineNumber { get; set; }
    }
}
