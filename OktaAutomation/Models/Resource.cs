using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;

namespace OktaAutomation.Models
{
    public class Resource
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("pos")]
        public Position Position { get; set; }

        public string ProductName 
        {
            get 
            {
                return Path.GetFileName(this.Position.FileName)
                            .Replace(".tf", string.Empty)
                            .Split("-")[1];
            }
        }
    }
}
