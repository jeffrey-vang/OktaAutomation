using Newtonsoft.Json;

namespace OktaAutomation.Models
{
    public class ManagedResources
    {
        [JsonProperty("managed_resources")]
        public Dictionary<string, Resource> Resources { get; set; }
    }
}
