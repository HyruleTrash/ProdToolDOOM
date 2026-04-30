

using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using ProdToolDOOM.ProjectFeatures;
using ProdToolDOOM.ProjectFeatures.Exporting;
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
    private StackPanel inProjectStackLeft = null!;
    private StackPanel inProjectStackRight = null!;
    private readonly ProjectFeatureInstance[] inProjectFeatures;
    public ContainerRuntime ToolContainer { get; private set; }
    private readonly ToolBarFeature toolBar;
    public ContainerRuntime canvasContainer = null!;
    public ContainerRuntime popUpContainer = null!;
    private readonly GumService gum;

    private class ProjectFeatureInstance(ProjectFeature instance, bool isLeft)
    {
        public ProjectFeature instance { get; } = instance;
        public bool isLeft { get; } = isLeft;
    }

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
            new ProjectFeatureInstance(new SaveFeature(this),  true),
            new ProjectFeatureInstance(new ExportFeature(this),  true),
            new ProjectFeatureInstance(new SwitchLevelFeature(this),  false),
            new ProjectFeatureInstance(new EntityDataManageFeature(this), false)
        ];
        this.toolBar = new ToolBarFeature(gum, this);
    }

    /// <summary>
    /// Checks the state of the current load strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    public bool CheckLoadStrategy()
    {
        this.loadStrat ??= new ProjectLoadStrategy();
        return this.loadStrat == null;
    }

    /// <summary>
    /// Checks the state of the current save strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    public bool CheckSaveStrategy()
    {
        this.saveStrat ??= new ProjectSaveStrategy();
        return this.saveStrat == null;
    }

    public void LoadUI(StackPanel mainPanel)
    {
        foreach (ProjectFeature projectFeature in this.projectFeatures)
            projectFeature.LoadUI(mainPanel);

        this.inProjectStackLeft = new StackPanel { IsVisible = false };
        this.inProjectStackRight = new StackPanel { IsVisible = false };
        mainPanel.AddChild(this.inProjectStackLeft);
        Program.instance.TopBarRight.AddChild(this.inProjectStackRight);
        
        this.filePathChanged += (newPath) =>
        {
            bool state = newPath != string.Empty;
            this.inProjectStackLeft.IsVisible = state;
            this.inProjectStackRight.IsVisible = state;
        };

        // all level objects will go here
        this.canvasContainer = new ContainerRuntime
        {
            Width = this.gum.CanvasWidth,
            Height = this.gum.CanvasHeight,
            X = 0,
            Y = 0,
        };
        this.canvasContainer.AddToRoot();
        
        // used by extra screens
        this.popUpContainer = new ContainerRuntime
        {
            Width = this.gum.CanvasWidth,
            Height = this.gum.CanvasHeight,
            X = 0,
            Y = 0,
        };
        this.popUpContainer.AddToRoot();

        // Tools
        this.ToolContainer = new ContainerRuntime
        {
            Width = this.gum.CanvasWidth,
            Height = this.gum.CanvasHeight,
            X = 0,
            Y = 0,
            Visible = false
        };
        this.filePathChanged += (newPath) => { this.ToolContainer.Visible = newPath != string.Empty; };
        this.ToolContainer.AddToRoot();
        
        Program.instance.onScreenSizeChange += size =>
        {
            this.canvasContainer.Width = size.x;
            this.canvasContainer.Height = size.y;
            this.popUpContainer.Width = size.x;
            this.popUpContainer.Height = size.y;
            this.ToolContainer.Width = size.x;
            this.ToolContainer.Height = size.y;
        };
        
        foreach (ProjectFeatureInstance projectFeature in this.inProjectFeatures.Where(i => i.isLeft)) 
            projectFeature.instance.LoadUI(this.inProjectStackLeft);
        foreach (ProjectFeatureInstance projectFeature in this.inProjectFeatures.Where(i => !i.isLeft)) 
            projectFeature.instance.LoadUI(this.inProjectStackRight);

        this.toolBar.LoadUI(this.ToolContainer);
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