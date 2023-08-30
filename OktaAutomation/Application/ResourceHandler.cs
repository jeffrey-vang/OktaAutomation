using Newtonsoft.Json;
using OktaAutomation.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OktaAutomation.Application
{
    public class ResourceHandler
    {
        public ManagedResources InspectResources(string modulePath, string outputPath)
        {
            var configExists = File.Exists(outputPath);
            if (!configExists)
            {
                var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "terraform-config-inspect.exe");
                var outputStream = new StreamWriter(outputPath);
                var inspectProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = $"--json {modulePath}",
                        RedirectStandardOutput = true,
                    }
                };
                inspectProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        outputStream.WriteLine(e.Data);
                    }
                });

                inspectProcess.Start();
                inspectProcess.BeginOutputReadLine();
                inspectProcess.WaitForExit();

                inspectProcess.Close();
                outputStream.Close();
            }

            var moduleConfig = File.ReadAllText(outputPath);
            return JsonConvert.DeserializeObject<ManagedResources>(moduleConfig);
        }
    }
}
