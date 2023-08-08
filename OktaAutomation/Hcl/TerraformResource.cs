namespace OktaAutomation.Hcl
{
    public class TerraformResource
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public Dictionary<string, object> Attributes { get; set; }

        public Dictionary<string, object> Lifecycle { get; set; }
    }
}
