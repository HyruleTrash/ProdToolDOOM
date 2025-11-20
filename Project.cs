

namespace ProdToolDOOM;

public class Project
{
    private Dictionary<int, EntityData> entityData;
    private List<Level> levels;
    private static IProjectSaveAndLoadStrategy saveAndLoadStrat = null;
    private static string filePath;

    public static void Load()
    {
        if (saveAndLoadStrat == null)
        {
            #if WINDOWS
            saveAndLoadStrat = new WindowsProjectSaveAndLoadStrategy();
            #endif
        }
        
        if (saveAndLoadStrat == null)
            return;
        Debug.Log("Loading project file...");
        filePath = FileExplorerHelper.OpenFileExplorer();
        Debug.Log(filePath);
        saveAndLoadStrat.Load(filePath);
    }
}