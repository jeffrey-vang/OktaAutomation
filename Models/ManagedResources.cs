using Newtonsoft.Json;

namespace OktaRoutingAutomation.Models
{
    public class ManagedResources
    {
        [JsonProperty("managed_resources")]
        public Dictionary<string, Resource> Resources { get; set; }
    }
}
