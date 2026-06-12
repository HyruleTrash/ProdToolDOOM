
namespace DLLevelBuilder;

public static class VersionManager
{
    // private static IProjectLoadStrategy versionOne = new Version1.ProjectLoadStrategy();
    private static readonly IProjectLoadStrategy VersionTwo = new Version2.ProjectLoadStrategy();
    private static readonly IProjectLoadStrategy VersionThree = new Version3.ProjectLoadStrategy();
    
    public static bool LoadUsingOldStrategy(string version, string filepath)
    {
        return version switch
        {
            // "0.0.1" => versionOne.Load(filepath),
            "0.0.2" => VersionTwo.Load(filepath),
            "0.0.3" => VersionThree.Load(filepath),
            _ => false
        };
    }
}