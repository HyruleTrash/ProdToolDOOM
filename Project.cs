

using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using ProdToolDOOM.Version1;
using Button = Gum.Forms.Controls.Button;

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
    private static Button loadProjectButton;
    private static Button saveProjectAsButton;
    
    private static StackPanel inProjectStack;
    private static Button saveProjectButton;

    private static ContainerRuntime toolContainer;
    private static StackPanel toolStack;
    private static Button addLevelButton;
    private static Button addEntityButton;
    private static Button addEntityDataButton;

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
            loadStrat = new ProjectLoadStrategy();
        
        return loadStrat == null;
    }
    
    /// <summary>
    /// Checks the state of the current save strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    private static bool CheckSaveStrategy()
    {
        if (saveStrat == null)
            saveStrat = new ProjectSaveStrategy();
        
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

    public static void LoadUI(StackPanel mainPanel, GumService gum)
    {
        loadProjectButton = new Button
        {
            Text = "Load Project"
        };
        loadProjectButton.Click += (sender, args) => Load();
        mainPanel.AddChild(loadProjectButton);
        
        saveProjectAsButton = new Button
        {
            Text = "New Project"
        };
        saveProjectAsButton.Click += (sender, args) => Save(true);
        filePathChanged += (string value) => {saveProjectAsButton.Text = value == String.Empty ? "New Project" : "Save Project as..."; };
        mainPanel.AddChild(saveProjectAsButton);
        
        inProjectStack = new StackPanel
        {
            IsVisible = false
        };
        filePathChanged += (string value) => { inProjectStack.IsVisible = value != String.Empty; };
        mainPanel.AddChild(inProjectStack);
        
        saveProjectButton = new Button
        {
            Text = "Save Project"
        };
        saveProjectButton.Click += (sender, args) => Save(false);
        inProjectStack.AddChild(saveProjectButton);
        
        // Tools
        toolContainer = new ContainerRuntime
        {
            Width = gum.CanvasWidth,
            Height = gum.CanvasHeight,
            X = 0,
            Y = 0,
            Visible = false
        };
        filePathChanged += (string value) => { toolContainer.Visible = value != String.Empty; };
        toolContainer.Anchor(Anchor.Center);
        toolContainer.AddToRoot();

        toolStack = new StackPanel
        {
            Visual =
            {
                ChildrenLayout = Gum.Managers.ChildrenLayout.LeftToRightStack,
                StackSpacing = 4
            },
            X = 5,
            Y = gum.CanvasHeight - UIParams.borderPadding,
        };
        toolStack.Anchor(Anchor.BottomLeft);
        toolContainer.AddChild(toolStack);

        // TODO: turn these into commands
        addLevelButton = new Button
        {
            Text = "Create new level"
        };
        addLevelButton.Click += (sender, args) => AddLevel();
        toolStack.AddChild(addLevelButton);
        
        addLevelButton = new Button
        {
            Text = "Add new Entity to project"
        };
        addLevelButton.Click += (sender, args) => AddEntityData();
        toolStack.AddChild(addLevelButton);
        
        addLevelButton = new Button
        {
            Text = "Add Entity to level"
        };
        addLevelButton.Click += (sender, args) => AddEntity();
        toolStack.AddChild(addLevelButton);
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
        if (entityDatas == null || entityDatas.Count == 0 || 
            levels == null || levels.Count == 0 || 
            currentLevel > levels.Count - 1)
            return;
        Debug.Log($"Adding entity to level{currentLevel}!");
        levels[currentLevel].Entities.Add(new Entity(0));
    }

    public static void ResetData()
    {
        levels = new();
        entityDatas = new();
        currentLevel = 0;
    }
}