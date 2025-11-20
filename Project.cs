

namespace ProdToolDOOM;

public class Project
{
    private Dictionary<int, EntityData> entityData;
    private List<Level> levels;
    private static IProjectSaveAndLoadStrategy saveAndLoadStrat = null;
    private static string filePath = string.Empty;

    public static void Load()
    {
        if (CheckStrategy())
            return;
        Debug.Log("Loading project file...");
        
        String tempPath = filePath;
        if (tempPath == String.Empty)
        {
            tempPath = FileExplorerHelper.OpenFileExplorer();
            Debug.Log(tempPath);
        }
        else
            tempPath = FileExplorerHelper.OpenFileExplorer(tempPath);
        if (saveAndLoadStrat.Load(tempPath))
            filePath = tempPath;
    }

    /// <summary>
    /// Checks the state of the current strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    private static bool CheckStrategy()
    {
        if (saveAndLoadStrat == null)
        {
            #if WINDOWS
            saveAndLoadStrat = new WindowsProjectSaveAndLoadStrategy();
            #endif
        }
        
        return saveAndLoadStrat == null;
    }

    public static void Save()
    {
        if (CheckStrategy())
            return;
        Debug.Log("Saving project file...");

        String tempPath = filePath;
        if (filePath == String.Empty)
        {
            tempPath = FileExplorerHelper.SaveWithFileExplorer();
            Debug.Log(tempPath);
        }
        else
            tempPath = FileExplorerHelper.SaveWithFileExplorer(filePath);
        if (saveAndLoadStrat.Save(tempPath))
            filePath = tempPath;
    }
}