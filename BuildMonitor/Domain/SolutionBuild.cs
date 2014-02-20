using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace BuildMonitor.Domain
{
    public class SolutionBuild : Build, ISolutionBuild
    {
        private readonly IList<IProjectBuild> projects;
        public ISolution Solution { get; private set; }
        private string configuration;

        public void AddProject(IProjectBuild projectBuild)
        {
            projects.Add(projectBuild);
        }

        public SolutionBuild(ITimer timer, string configuration, ISolution solution) : base(timer)
        {
            Solution = solution;
            this.configuration = configuration;
            projects = new List<IProjectBuild>();
        }

        private object BuildUserInfo()
        {
            var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
            return new
            {
                MachineName = System.Environment.MachineName,
                OsVersion = System.Environment.OSVersion.VersionString,
                ProcessorCount = System.Environment.ProcessorCount,
                UserDomainName = System.Environment.UserDomainName,
                UserInteractive = System.Environment.UserInteractive,
                UserName = System.Environment.UserName,
                ClrVersion = System.Environment.Version.ToString(4),
                AvailablePhysicalMemory = ci.AvailablePhysicalMemory,
                AvailableVirtualMemory = ci.AvailableVirtualMemory,
                TotalPhysicalMemory = ci.TotalPhysicalMemory,
                TotalVirtualMemory = ci.TotalVirtualMemory,
            };
        }

        public object Data()
        {
            return new
            {
                UserInfo = BuildUserInfo(),
                Start = Started,
                Time = MillisecondsElapsed,
                Configuration = configuration,
                Solution,
                Projects = projects.Select(p => p.Data())
            };
        }
    }
}