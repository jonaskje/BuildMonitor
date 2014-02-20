using BuildMonitor.Domain;

namespace BuildMonitor.UnitTests.Fakes
{
    internal class BuildFactoryFake : IBuildFactory
    {
        public ISolutionBuild Build { get; set; }

        public ISolutionBuild CreateSolutionBuild(string configuration, ISolution solution)
        {
            return Build;
        }

        public IProjectBuild CreateProjectBuild(IProject project)
        {
            throw new System.NotImplementedException();
        }
    }
}