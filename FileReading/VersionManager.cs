
namespace ProdToolDOOM;

public static class VersionManager
{
    private static IProjectLoadStrategy versionOne = new Version1.ProjectLoadStrategy();
    private static IProjectLoadStrategy versionTwo = new Version2.ProjectLoadStrategy();
    
    public static bool LoadUsingOldStrategy(string version, string filepath)
    {
        return version switch
        {
            "0.0.1" => versionOne.Load(filepath),
            "0.0.2" => versionTwo.Load(filepath),
            _ => false
        };
    }
}