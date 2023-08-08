using OktaAutomation.Enums;
using OktaAutomation.Extensions;
using OktaAutomation.Models;
using System.Text.RegularExpressions;

namespace OktaAutomation.Application
{
    public class RedirectHandler
    {
        public Result ApplyRedirect(Enums.Environment env, Resource resource, int offset)
        {
            // Read File
            var lines = File.ReadAllLines(resource.Position.FileName);
            var orignalLines = lines.ToList();

            var position = resource.Position.LineNumber + offset - 1;
            var resourceBlock = orignalLines.SelectResource(position);

            // Fetch Uri
            var component = GetComponentName(resourceBlock);
            var uri = GetRedirectUri(env, component);

            // Append Uri to Line
            if (resourceBlock.Any(x => x.Contains(uri)))
            {
                return Result.Skipped;
            }
            else
            {
                if (!resourceBlock.Any())
                {
                    return Result.Failed;
                }

                var lineMark = 0;
                foreach (var line in resourceBlock)
                {
                    // If redirect uris are on a single line
                    if (!line.Contains("ignore_changes") && line.Contains("redirect_uris") && line.Contains("=") && line.Contains("[") && line.Contains("]"))
                    {
                        // Only update if uri already exists
                        if (!line.Contains(uri))
                        {
                            var originalRedirectLine = line;
                            var newRedirectLine = line.Replace("]", $",\"{uri}\" ]");

                            var redirectIndex = position + lineMark;
                            orignalLines[redirectIndex] = line.Replace(originalRedirectLine, newRedirectLine);

                            // Write Output
                            File.WriteAllLines(resource.Position.FileName, orignalLines);

                            Console.WriteLine($"Appending redirect for {resource.Name} in {resource.Position.FileName}");
                            return Result.Success;
                        }
                    }
                    // Redirects are on multiple lines
                    else if (line.Contains("redirect_uris") && line.Contains("[") && !line.Contains("]"))
                    {
                        var redirectIndex = resourceBlock.FindIndex(0, resourceBlock.Count, x => x.Contains("redirect_uris"));
                        orignalLines.Insert(position + redirectIndex + 1, $"    \"{uri}\",");
                        offset++;

                        // Write Output
                        File.WriteAllLines(resource.Position.FileName, orignalLines);

                        Console.WriteLine($"Appending redirect for {resource.Name} in {resource.Position.FileName}");
                        return Result.Success;
                    }
                    lineMark++;
                }
            }
            return Result.Failed;
        }

        private static string GetRedirectUri(Enums.Environment env, string component, string system = "defaultsystem")
        {
            return $"https://{env.ToRoutingPrefix()}{component}.{system}.chr.io/openapi/oauth2-redirect.html";
        }

        private static string GetComponentName(List<string> resourceBlock)
        {
            var pattern = new Regex("\"(.*?)\"");
            var label = resourceBlock.First(x => x.Contains("label", StringComparison.OrdinalIgnoreCase));
            var match = pattern.Match(label);

            if (!match.Success)
            {
                throw new Exception("No Component Name");
            }

            var labelSplit = match.Value.Split('-');
            var componentName = labelSplit.Length == 1 ? labelSplit[0] : labelSplit[1];

            return componentName

                // remove double quotes
                .Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase)

                // remove swagger reference
                .Replace(".SwaggerUI", string.Empty, StringComparison.OrdinalIgnoreCase)

                // replace dots with dashes
                .Replace(".", "-", StringComparison.OrdinalIgnoreCase)
               
                // Lowercase the component name
                .ToLower();
        }
    }
}
