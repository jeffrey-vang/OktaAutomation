using OktaAutomation.Application;
using OktaAutomation.Enums;
using OktaAutomation.Extensions;

namespace OktaAutomation
{
    public class Program
    {
        static int writeCount = 0;
        static int resourcesSkipped = 0;
        static int failedResources = 0;

        public static void Main(string[] args)
        {
            var redirectHandler = new RedirectHandler();
            var resourceHandler = new ResourceHandler();

            //var repo = "JVOKTA"; 
             var repo = "okta-terraform-config";

            var module = "nonprod";
            //var module = "prod";

            // Filters
            var environmentFilter = Enums.Environment.Integration;
            var resourceFilter = "okta_app_oauth";
            var nameFilter = "SwaggerUI";
            var domainFilter = "";
            var productFilter = "Money.Tax.Api";


            Console.WriteLine($"Running against {module}...");

            var modulePath = $"C:\\TP\\Okta\\{repo}\\{module}";
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"{module}_{repo}_tf.json");

            // Inspect Resources
            var managedResources = resourceHandler.InspectResources(modulePath, outputPath);
            var swaggerResources = managedResources.Resources.Values.Where(x =>
                x.Type == resourceFilter
                && x.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)
                && (!string.IsNullOrEmpty(productFilter) && x.Position.FileName.Contains(productFilter, StringComparison.OrdinalIgnoreCase))
                && (!string.IsNullOrEmpty(domainFilter) && x.Position.FileName.Contains(domainFilter, StringComparison.OrdinalIgnoreCase))
                && x.Position.FileName.Contains(environmentFilter.ToRoutingPrefix(), StringComparison.OrdinalIgnoreCase));

            var groupedResources = swaggerResources.OrderBy(x => x.Position.LineNumber).GroupBy(x => x.Position.FileName);

            // Process groups with multiple resources in a single file - Order by line number.
            foreach (var resourceGroup in groupedResources)
            {
                var offset = 0;
                foreach (var resource in resourceGroup)
                {
                    Console.WriteLine($"Appending redirect for {resource.Name} in {resource.Position.FileName}");
                    var result = redirectHandler.ApplyRedirect(environmentFilter, resource, offset);

                    if (result == Result.Success)
                    {
                        writeCount++;
                    }

                    if (result == Result.Failed)
                    {
                        failedResources++;
                    }

                    if (result == Result.Skipped)
                    {
                        resourcesSkipped++;
                    }
                }
            }


            Console.WriteLine($"Write Count: {writeCount}");
            Console.WriteLine($"Resource Count: {swaggerResources.Count()}");
            Console.WriteLine($"Failed Resources: {failedResources}");
            Console.WriteLine($"Resources Skipped Because Uri Already Exists: {resourcesSkipped}");
            Console.WriteLine($"Files With Resources (Expected Files Changed): {groupedResources.Count()}");
        }
    }
}