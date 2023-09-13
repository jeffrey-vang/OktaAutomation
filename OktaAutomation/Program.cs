using OktaAutomation.Application;
using OktaAutomation.Enums;
using OktaAutomation.Extensions;
using System.Linq;

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

            var productList = File.ReadAllLines("Products.txt");
            var allProducts = new Dictionary<string, string>();
            foreach (var product in productList)
            {
                var productResult = product.Split(";");
                allProducts.Add(productResult[0], productResult[1]);
            }

            //var repo = "JVOKTA"; 
             var repo = "okta-terraform-config";

            //var module = "nonprod";
            var module = "prod";

            // Filters
            var environmentFilter = Enums.Environment.Production;
            var resourceFilter = "okta_app_oauth";
            var nameFilter = "SwaggerUI";
            var domainFilter = "";
            var productFilter = true;


            Console.WriteLine($"Running against {module}...");

            var modulePath = $"C:\\TP\\Okta\\{repo}\\{module}";
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"{module}_{repo}_tf.json");

            // Inspect Resources
            var managedResources = resourceHandler.InspectResources(modulePath, outputPath);
            var resources = managedResources.Resources.Values.Where(x => x.Type == resourceFilter);

            var swaggerResources = resources.Where(x => x.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
            var environmentResources = swaggerResources.Where(x => 
                x.Position.FileName.Contains(environmentFilter.ToRoutingPrefix(), StringComparison.OrdinalIgnoreCase));
            var productResources = swaggerResources.Where(x =>
                CheckProduct(allProducts.Keys.ToArray(), x.Position.FileName, environmentFilter.ToPrefix())).ToList();

            var groupedResources = productResources.OrderBy(x => x.Position.LineNumber).GroupBy(x => x.Position.FileName);

            // Missed Resources
            var productNames = productResources
                .Select(x => GetResourceProductName(x.Position.FileName)).ToList();
            var missedResources = allProducts.Keys.Where(x => !productNames.Contains(x)).ToList();

            // Process groups with multiple resources in a single file - Order by line number.
            foreach (var resourceGroup in groupedResources)
            {
                var offset = 0;
                foreach (var resource in resourceGroup)
                {
                    Console.WriteLine($"Appending redirect for {resource.Name} in {resource.Position.FileName}");

                    var resourceProductName = GetResourceProductName(resource.Position.FileName);
                    var redirect = allProducts.First(x => x.Key.Contains(resourceProductName)).Value;
                    var envRedirect = $"https://{environmentFilter.ToRoutingPrefix()}{redirect}/openapi/oauth2-redirect.html";

                    var result = redirectHandler.ApplyRedirect(environmentFilter, resource, offset, envRedirect);

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
            Console.WriteLine($"Missed Products Count: {missedResources.Count()}");

            foreach(var resource in missedResources)
            {
                Console.WriteLine($"Missed Product: {resource}");
            }


            Console.WriteLine($"Resource Count: {swaggerResources.Count()}");
            Console.WriteLine($"Failed Resources: {failedResources}");
            Console.WriteLine($"Resources Skipped Because Uri Already Exists: {resourcesSkipped}");
            Console.WriteLine($"Files With Resources (Expected Files Changed): {groupedResources.Count()}");
        }

        private static bool CheckProduct(string[] products, string filePath, string env)
        {
            var fileNames = products.Select(x => $"{env}-{x}.tf").ToList();
            var fileName = Path.GetFileName(filePath);

            return fileNames.Contains(fileName);
        }

        private static string GetResourceProductName(string resourcePath)
        {
            return Path.GetFileName(resourcePath)
                        .Replace(".tf", string.Empty)
                        .Split("-")[1];
        }
    }
}