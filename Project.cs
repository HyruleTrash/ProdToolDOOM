

using Gum.Forms.Controls;
using Button = Gum.Forms.Controls.Button;

using WindowsProjectLoadStrategy = ProdToolDOOM.Version1.WindowsProjectLoadStrategy;
using WindowsProjectSaveStrategy = ProdToolDOOM.Version1.WindowsProjectSaveStrategy;

namespace ProdToolDOOM;

public static class Project
{
    public static Dictionary<int, EntityData> entityDatas = [];
    public static int idCounter = 0;
    public static List<Level> levels = [];
    public static int currentLevel = 0;
    private static IProjectSaveStrategy saveStrat = null;
    private static IProjectLoadStrategy loadStrat = null;

    private static string FilePath
    {
        get => filePath;
        set
        {
            if (filePath != value)
                filePathChanged.Invoke(value);
            filePath = value;
        }
    }
    private static string filePath = string.Empty;
    private static Action<string> filePathChanged;
    private static Button LoadProjectButton;
    private static Button SaveProjectAsButton;
    
    private static StackPanel inProjectStack;
    private static Button SaveProjectButton;
    private static Button AddLevelButton;
    private static Button AddEntityButton;
    private static Button AddEntityDataButton;

    static Project()
    {
        filePathChanged += (string value) => { Debug.Log($"FilePathChanged: {value}"); };
    }

    private static void Load()
    {
        if (CheckLoadStrategy())
            return;
        Debug.Log("Loading project file...");
        
        String tempPath = FilePath;
        if (tempPath == String.Empty)
        {
            tempPath = FileExplorerHelper.OpenFileExplorer();
            Debug.Log(tempPath);
        }
        else
            tempPath = FileExplorerHelper.OpenFileExplorer(tempPath);
        if (loadStrat.Load(tempPath))
            FilePath = tempPath;
    }

    /// <summary>
    /// Checks the state of the current load strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    private static bool CheckLoadStrategy()
    {
        if (loadStrat == null)
        {
            #if WINDOWS
            loadStrat = new WindowsProjectLoadStrategy();
            #endif
        }
        
        return loadStrat == null;
    }
    
    /// <summary>
    /// Checks the state of the current save strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    private static bool CheckSaveStrategy()
    {
        if (saveStrat == null)
        {
            #if WINDOWS
            saveStrat = new WindowsProjectSaveStrategy();
            #endif
        }
        
        return saveStrat == null;
    }

    private static void Save(bool newProject = false)
    {
        if (CheckSaveStrategy())
            return;
        Debug.Log("Saving project file...");

        String tempPath = FilePath;
        if (newProject)
        {
            if (FilePath == String.Empty)
            {
                tempPath = FileExplorerHelper.SaveWithFileExplorer();
                Debug.Log(tempPath);
            }
            else
                tempPath = FileExplorerHelper.SaveWithFileExplorer(tempPath);
        }
        else
        {
            if (FilePath == String.Empty)
                return;
        }

        if (saveStrat.Save(tempPath))
            FilePath = tempPath;
    }

    public static void LoadUI(StackPanel mainPanel)
    {
        LoadProjectButton = new Button
        {
            Text = "Load Project"
        };
        LoadProjectButton.Click += (sender, args) => Load();
        mainPanel.AddChild(LoadProjectButton);
        
        SaveProjectAsButton = new Button
        {
            Text = "New Project"
        };
        SaveProjectAsButton.Click += (sender, args) => Save(true);
        filePathChanged += (string value) => {SaveProjectAsButton.Text = value == String.Empty ? "New Project" : "Save Project as..."; };
        mainPanel.AddChild(SaveProjectAsButton);
        
        inProjectStack = new StackPanel
        {
            IsVisible = false
        };
        filePathChanged += (string value) => { inProjectStack.IsVisible = value != String.Empty; };
        mainPanel.AddChild(inProjectStack);
        
        SaveProjectButton = new Button
        {
            Text = "Save Project"
        };
        SaveProjectButton.Click += (sender, args) => Save(false);
        inProjectStack.AddChild(SaveProjectButton);
        
        AddLevelButton = new Button
        {
            Text = "Create new level"
        };
        AddLevelButton.Click += (sender, args) => AddLevel();
        inProjectStack.AddChild(AddLevelButton);
        
        AddLevelButton = new Button
        {
            Text = "Add new Entity to project"
        };
        AddLevelButton.Click += (sender, args) => AddEntityData();
        inProjectStack.AddChild(AddLevelButton);
        
        AddLevelButton = new Button
        {
            Text = "Add Entity to level"
        };
        AddLevelButton.Click += (sender, args) => AddEntity();
        inProjectStack.AddChild(AddLevelButton);
    }

    private static void AddLevel()
    {
        Debug.Log("Adding level!");
        levels.Add(new Level());
        currentLevel = levels.Count - 1;
    }

    private static void AddEntityData()
    {
        Debug.Log("Adding entity data!");
        entityDatas.Add(idCounter, new EntityData());
        idCounter++;
    }

    private static void AddEntity()
    {
        if (entityDatas == null ||  entityDatas.Count == 0)
            return;
        Debug.Log("Adding entity!");
        levels[currentLevel].Entities.Add(new Entity(0));
    }

    public static void ResetData()
    {
        levels = new();
        entityDatas = new();
        currentLevel = 0;
    }
}