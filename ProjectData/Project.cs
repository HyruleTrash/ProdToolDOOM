

using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using ProdToolDOOM.ProjectFeatures;
using ProdToolDOOM.Version2;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM;

public class Project
{
    public static Project instance => Program.instance.currentProject;
    // file reading
    public IProjectSaveStrategy? saveStrat;
    public IProjectLoadStrategy? loadStrat;
    public string FilePath
    {
        get => this.filePath;
        set
        {
            if (this.filePath != value) this.filePathChanged.Invoke(value);
            this.filePath = value;
        }
    }
    private string filePath = string.Empty;
    public Action<string> filePathChanged;
    // data
    public Dictionary<int, EntityData> entityDatas = [];
    public int entityDataIdCounter = 0;
    public List<Level> levels = [];

    public int CurrentLevel
    {
        get => this.currentLevel;
        set
        {
            if (value < -1 || value >= this.levels.Count)
                return;
            if (this.currentLevel != value) this.onCurrentLevelChanged?.Invoke(value);
            this.currentLevel = value;
        }
    }
    private int currentLevel = -1;
    public Action<int>? onCurrentLevelChanged;
    
    // UI
    private readonly ProjectFeature[] projectFeatures;
    private StackPanel inProjectStack = null!;
    private readonly ProjectFeature[] inProjectFeatures;
    private ContainerRuntime toolContainer = null!;
    private ToolBarFeature toolBar;
    public ContainerRuntime canvasContainer = null!;
    private readonly GumService gum;

    public Project(GumService gum)
    {
        this.gum = gum;
        this.filePathChanged = newPath => { Debug.Log($"FilePathChanged: {newPath}"); };
        this.projectFeatures =
        [
            new LoadFeature(this), 
            new SaveNewFeature(this)
        ];
        this.inProjectFeatures =
        [
            new SaveFeature(this),
            new SwitchLevelFeature(this),
        ];
        this.toolBar = new ToolBarFeature(gum, this);
    }

    /// <summary>
    /// Checks the state of the current load strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    public bool CheckLoadStrategy()
    {
        if (this.loadStrat == null) this.loadStrat = new ProjectLoadStrategy();

        return this.loadStrat == null;
    }

    /// <summary>
    /// Checks the state of the current save strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    public bool CheckSaveStrategy()
    {
        if (this.saveStrat == null) this.saveStrat = new ProjectSaveStrategy();

        return this.saveStrat == null;
    }

    public void LoadUI(StackPanel mainPanel)
    {
        foreach (ProjectFeature projectFeature in this.projectFeatures)
            projectFeature.LoadUI(mainPanel);

        this.inProjectStack = new StackPanel
        {
            IsVisible = false
        };
        this.filePathChanged += (newPath) => { this.inProjectStack.IsVisible = newPath != string.Empty; };
        mainPanel.AddChild(this.inProjectStack);
        
        foreach (ProjectFeature projectFeature in this.inProjectFeatures) 
            projectFeature.LoadUI(this.inProjectStack);

        this.canvasContainer = new ContainerRuntime
        {
            Width = this.gum.CanvasWidth,
            Height = this.gum.CanvasHeight,
            X = 0,
            Y = 0,
        };
        this.canvasContainer.AddToRoot();

        // Tools
        this.toolContainer = new ContainerRuntime
        {
            Width = this.gum.CanvasWidth,
            Height = this.gum.CanvasHeight,
            X = 0,
            Y = 0,
            Visible = false
        };
        this.filePathChanged += (newPath) => { this.toolContainer.Visible = newPath != string.Empty; };
        this.toolContainer.AddToRoot();
        
        Program.instance.onScreenSizeChange += size =>
        {
            this.canvasContainer.Width = size.x;
            this.canvasContainer.Height = size.y;
            this.toolContainer.Width = size.x;
            this.toolContainer.Height = size.y;
        };

        this.toolBar.LoadUI(this.toolContainer);
    }

    public void ResetData()
    {
        this.levels = new();
        this.entityDatas = new();
        this.CurrentLevel = -1;
        Program.instance.cmdHistory.Reset();
        this.canvasContainer.Children?.Clear();
    }
}