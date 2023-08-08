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

            var repo = "JVOKTA"; 
            // var repo = "okta-terraform-config";

            var module = "nonprod";
            //var module = "prod";

            var environment = Enums.Environment.Development;

            Console.WriteLine($"Running against {module}...");

            var modulePath = $"C:\\TP\\Okta\\{repo}\\{module}";
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"{module}_{repo}_tf.json");

            // Inspect Resources
            var managedResources = resourceHandler.InspectResources(modulePath, outputPath);
            var swaggerResources = managedResources.Resources.Values.Where(x =>
                x.Type == "okta_app_oauth"
                && x.Name.Contains("SwaggerUI")
                && x.Position.FileName.Contains(environment.ToRoutingPrefix(), StringComparison.OrdinalIgnoreCase));

            var allGroupedResources = swaggerResources.OrderBy(x => x.Position.LineNumber).GroupBy(x => x.Position.FileName);
            var groupedResources = allGroupedResources.Where(x => x.Count() > 1);
            //var singleResources = allGroupedResources.Where(x => x.Count() == 1).SelectMany(x => x);

            // Process groups with single resources in a single file.
/*            foreach (var resource in singleResources)
            {
                var result = redirectHandler.ApplyRedirect(environment, resource, 0);

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
            }*/

            // Process groups with multiple resources in a single file - Order by line number.
            foreach (var resourceGroup in allGroupedResources)
            {
                var offset = 0;
                foreach (var resource in resourceGroup)
                {
                    var result = redirectHandler.ApplyRedirect(environment, resource, offset);

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
            Console.WriteLine($"Files With Resources (Expected Files Changed): {allGroupedResources.Count()}");
        }
    }
}