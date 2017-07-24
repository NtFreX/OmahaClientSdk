using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Omaha
{
    public class OmahaLogProvider
    {
        private Logger Logger { get; set; }
        private string LogFile { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OmahaLogProvider"/> class.
        /// </summary>
        /// <param name="companyName">Name of the company.</param>
        /// <param name="appName">Name of the application.</param>
        /// <param name="level">The loging level.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private OmahaLogProvider(string companyName, string appName, LogLevel level)
        {
            if(string.IsNullOrEmpty(companyName)) throw new ArgumentNullException(nameof(companyName));
            if(string.IsNullOrEmpty(appName)) throw new ArgumentNullException(nameof(appName));
            if (level == null) throw new ArgumentNullException(nameof(level));

            //throws PlatformNotSupportedException
            var logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), companyName + "\\" + appName);
            IoHelper.CreateDirectoryIfNotExists(logFilePath);
            LogFile = logFilePath + "\\log.txt";

            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = LogFile;
            fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

            var rule = new LoggingRule("*", level, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;

            Logger = LogManager.GetLogger("Omaha");
        }

        private static readonly Dictionary<string, OmahaLogProvider> Instances = new Dictionary<string, OmahaLogProvider>();
        public static Logger GetInstance(string companyName, string appName, LogLevel level)
        {
            var key = companyName + "\\" + appName;
            if (!Instances.ContainsKey(key))
                Instances.Add(key, new OmahaLogProvider(companyName, appName, level));
            return Instances[key].Logger;
        }

        public static Dictionary<string, string> GetLogFiles()
        {
            return Instances.ToDictionary(instance => instance.Key, instance => instance.Value.LogFile);
        }
    }
}
