namespace OktaAutomation.Hcl
{
    public class HclHelper
    {
        public TerraformResource ParseResource(string content)
        {
            var resource = new TerraformResource();

            var lines = content.Split('\n');

            var resoureceLine = lines.First(x => x.Contains("resource"));
            var sections = resoureceLine.Split(" ");
            resource.Type = sections[1].Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase);
            resource.Name = sections[2].Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase);

            var lifeCycleLine = lines.FirstOrDefault(x => x.Contains("lifecycle"));
            resource.Lifecycle = this.ParseAttributes(content, lifeCycleLine);

            var attributeLine = lines.First(x => !x.Contains("resource") && !x.Contains("lifecycle"));
            resource.Attributes = this.ParseAttributes(content, attributeLine);

            return resource;
        }

        public Dictionary<string, object> ParseAttributes(string content, string startLine)
        {
            var attributes = new Dictionary<string, object>();

            var lines = content.Split('\n');
            var index = Array.IndexOf(lines, startLine);

            var startingPoint = lines.Skip(index);

            foreach (var next in startingPoint)
            {
                if (next.Trim().Contains("="))
                {
                    var section = next.Split("=");

                    var key = section[0].Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
                    var value  = section[1].Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

                    if (value.Contains('[') && value.Contains("]"))
                    {
                        var sequence = value
                            .Replace("[", string.Empty)
                            .Replace("]", string.Empty)
                            .Replace(" ", string.Empty)
                            .Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase)
                            .Trim()
                            .Split(',');

                        attributes.Add(key, sequence);
                    }
                    else if (value.Contains('[') && !value.Contains("]"))
                    {
                        var sequence = new List<string>();
                        var multilineStartIndex = Array.IndexOf(lines, next);
                        var multiLineStart = lines.Skip(multilineStartIndex);
                        foreach (var multiLine in multiLineStart)
                        {
                            var nextLine = multiLine
                                .Replace(",", string.Empty)
                                .Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase)
                                .Trim();

                            if (!nextLine.Trim().Contains('[') && !nextLine.Trim().Contains(']'))
                            {
                                sequence.Add(nextLine);
                            }

                            if (nextLine.Contains(']'))
                            {
                                break;
                            }
                        }
                        attributes.Add(key, sequence);
                    }
                    else
                    {
                        attributes.Add(key, value.Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            return attributes;
        }
    }
}
