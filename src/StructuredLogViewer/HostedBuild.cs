﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.CommandLine;
using Microsoft.Build.Logging.StructuredLogger;

namespace StructuredLogViewer
{
    public class HostedBuild
    {
        private string projectFilePath;

        public HostedBuild(string projectFilePath)
        {
            this.projectFilePath = projectFilePath;
        }

        public Task<Build> BuildAndGetResult()
        {
            var msbuildExe = typeof(MSBuildApp).Assembly.Location;
            var loggerDll = typeof(StructuredLogger).Assembly.Location;
            var logXml = Path.Combine(Path.GetTempPath(), "StructuredLogger");
            logXml = Path.Combine(logXml, Environment.TickCount.ToString());
            var commandLine = $@"""{msbuildExe}"" ""{projectFilePath}"" /t:Rebuild /noconlog /logger:{nameof(StructuredLogger)},""{loggerDll}"";""{logXml}""";
            StructuredLogger.SaveLogToDisk = false;

            return System.Threading.Tasks.Task.Run(() =>
            {
                var result = MSBuildApp.Execute(commandLine);
                return StructuredLogger.CurrentBuild;
            });
        }
    }
}