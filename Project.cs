

using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using ProdToolDOOM.Version1;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM;

public class Project
{
    public static Project Instance => Program.instance.currentProject;

    public Dictionary<int, EntityData> entityDatas = [];
    public int idCounter = 0;
    
    public List<Level> levels = [];
    public int currentLevel = 0;
    
    private IProjectSaveStrategy saveStrat = null;
    private IProjectLoadStrategy loadStrat = null;

    private string FilePath
    {
        get => filePath;
        set
        {
            if (filePath != value)
                filePathChanged.Invoke(value);
            filePath = value;
        }
    }
    private string filePath = string.Empty;
    private Action<string> filePathChanged;
    private Button loadProjectButton;
    private Button saveProjectAsButton;
    
    private StackPanel inProjectStack;
    private Button saveProjectButton;

    private ContainerRuntime toolContainer;
    private StackPanel toolStack;
    private Button addLevelButton;
    private Button addEntityButton;
    private Button addEntityDataButton;

    public Project()
    {
        filePathChanged = (string value) => { Debug.Log($"FilePathChanged: {value}"); };
    }

    private void Load()
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
    private bool CheckLoadStrategy()
    {
        if (loadStrat == null)
            loadStrat = new ProjectLoadStrategy();
        
        return loadStrat == null;
    }
    
    /// <summary>
    /// Checks the state of the current save strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    private bool CheckSaveStrategy()
    {
        if (saveStrat == null)
            saveStrat = new ProjectSaveStrategy();
        
        return saveStrat == null;
    }

    private void Save(bool newProject = false)
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

    public void LoadUI(StackPanel mainPanel, GumService gum)
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

    private void AddLevel()
    {
        Debug.Log("Adding level!");
        levels.Add(new Level());
        currentLevel = levels.Count - 1;
    }

    private void AddEntityData()
    {
        Debug.Log("Adding entity data!");
        entityDatas.Add(idCounter, new EntityData());
        idCounter++;
    }

    private void AddEntity()
    {
        if (entityDatas == null || entityDatas.Count == 0 || 
            levels == null || levels.Count == 0 || 
            currentLevel > levels.Count - 1)
            return;
        Debug.Log($"Adding entity to level{currentLevel}!");
        levels[currentLevel].Entities.Add(new Entity(0));
    }

    public void ResetData()
    {
        levels = new();
        entityDatas = new();
        currentLevel = 0;
    }
}