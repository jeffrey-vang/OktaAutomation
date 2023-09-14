using OktaAutomation.Enums;
using OktaAutomation.Extensions;
using OktaAutomation.Models;

namespace OktaAutomation.Application
{
    public class GroupAssignmentHandler
    {
        private readonly string template;
        public GroupAssignmentHandler()
        {
            this.template = File.ReadAllText("GroupAssignmentTemplate.txt");
        }

        public Result ApplyGroup(Enums.Environment env, Resource resource, int offset)
        {
            // Read File
            var lines = File.ReadAllLines(resource.Position.FileName);
            var orignalLines = lines.ToList();

            var position = resource.Position.LineNumber + offset - 1;
            var resourceBlock = orignalLines.SelectResource(position);

            if (resourceBlock.Any(x => x.Contains("skip_groups")))
            {
                return Result.Skipped;
            }
            else
            {
                if (!resourceBlock.Any())
                {
                    return Result.Failed;
                }

                // Replace the groups assignment
                var lineMark = 0;
                foreach (var line in resourceBlock)
                {
                    // Remove the groups statement.
                    if (line.Contains("groups") && !line.Contains("skip_groups"))
                    {
                        // Remove group line.
                        var groupsIndex = position + lineMark;
                        orignalLines.RemoveAt(groupsIndex);
                    }
                    lineMark++;
                }

                // Add group assignment
                // Add new group assignment
                orignalLines[position + (lineMark - 2)] =
@"  skip_groups                = true
  skip_users                 = true
  lifecycle {
      ignore_changes = [users, groups]
  }
}";

                var assignmentBlock = this.template.Replace("{{ResourceName}}", resource.Name);
                orignalLines.Add(assignmentBlock);

                // Write Output
                File.WriteAllLines(resource.Position.FileName, orignalLines);
                return Result.Success;
            }

            return Result.Failed;
        }
    }
}
