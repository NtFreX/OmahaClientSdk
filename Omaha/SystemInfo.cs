using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

namespace Omaha
{
    public class SystemInfo
    {
        public static string GetSystemInfo(ExpandoObject additionalSystemInfo = null)
        {
            OmahaLogProvider.GetInstance(OmahaConstants.CompanyName, OmahaConstants.AppName, OmahaConstants.LogLevel).Info("generating system info object");

            dynamic systemInfo = new ExpandoObject();
            List<Assembly> loadedAssemblies = new List<Assembly>(new [] { Assembly.GetEntryAssembly() });
            loadedAssemblies.AddRange(loadedAssemblies[0].GetReferencedAssemblies().ToList().ConvertAll(Assembly.Load));
            foreach (var asssembly in loadedAssemblies)
            {
                foreach (var type in asssembly.GetTypes())
                {
                    SystemInfoAttribute.ReadSystemInfo(type, ref systemInfo);
                }
            }
            systemInfo.System = new ExpandoObject();
            systemInfo.System.Is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
            systemInfo.System.MachineName = Environment.MachineName;
            systemInfo.System.CurrentDirectory = Environment.CurrentDirectory;
            systemInfo.System.OperatingSystem = new ExpandoObject();
            systemInfo.System.OperatingSystem.Platform = Environment.OSVersion.Platform.ToString();
            systemInfo.System.OperatingSystem.ServicePack = Environment.OSVersion.ServicePack;
            systemInfo.System.OperatingSystem.Version = Environment.OSVersion.VersionString;
            systemInfo.System.ProcessorCount = Environment.ProcessorCount;
            systemInfo.System.Environment = Environment.GetEnvironmentVariables();
            systemInfo.System.ClrVersion = Environment.Version.ToString();

            systemInfo.AdditionalSystemInfo = additionalSystemInfo ?? new ExpandoObject();

            return JsonConvert.SerializeObject((IDictionary<string, Object>)systemInfo, Formatting.Indented);
        }
    }
}
