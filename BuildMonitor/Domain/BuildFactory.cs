namespace BuildMonitor.Domain
{
    public class BuildFactory : IBuildFactory
    {
        public ISolutionBuild CreateSolutionBuild(string configuration, ISolution solution)
        {
            return new SolutionBuild(new BuildTimer(), configuration, solution);
        }

        public IProjectBuild CreateProjectBuild(IProject project)
        {
            return new ProjectBuild(new BuildTimer(), project);
        }
    }
}