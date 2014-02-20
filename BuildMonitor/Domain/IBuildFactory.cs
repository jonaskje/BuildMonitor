namespace BuildMonitor.Domain
{
    public interface IBuildFactory
    {
        ISolutionBuild CreateSolutionBuild(string configuration, ISolution solution);
        IProjectBuild CreateProjectBuild(IProject project);
    }
}