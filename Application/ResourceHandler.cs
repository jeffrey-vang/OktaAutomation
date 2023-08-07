using Newtonsoft.Json;
using OktaRoutingAutomation.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OktaRoutingAutomation.Application
{
    public class ResourceHandler
    {
        public ManagedResources InspectResources(string modulePath, string outputPath)
        {
            var configExists = File.Exists(outputPath);
            if (!configExists)
            {
                var outputStream = new StreamWriter(outputPath);
                var inspectProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "terraform-config-inspect",
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
