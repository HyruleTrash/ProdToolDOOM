

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
        get => filePath;
        set
        {
            if (filePath != value)
                filePathChanged.Invoke(value);
            filePath = value;
        }
    }
    private string filePath = string.Empty;
    public Action<string> filePathChanged;
    // data
    public Dictionary<int, EntityData> entityDatas = [];
    public int idCounter = 0;
    public List<Level> levels = [];
    public int currentLevel = 0;
    // UI
    private readonly ProjectFeature[] projectFeatures;
    private StackPanel inProjectStack = null!;
    private readonly ProjectFeature[] inProjectFeatures;
    private ContainerRuntime toolContainer = null!;
    private ToolBarFeature toolBar = null!;

    public Project()
    {
        filePathChanged = newPath => { Debug.Log($"FilePathChanged: {newPath}"); };
        projectFeatures =
        [
            new LoadFeature(this), 
            new SaveNewFeature(this)
        ];
        inProjectFeatures =
        [
            new SaveFeature(this)
        ];
    }

    /// <summary>
    /// Checks the state of the current load strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    public bool CheckLoadStrategy()
    {
        if (loadStrat == null)
            loadStrat = new ProjectLoadStrategy();

        return loadStrat == null;
    }

    /// <summary>
    /// Checks the state of the current save strategy
    /// </summary>
    /// <returns>true if strategy is unset, false if it is set</returns>
    public bool CheckSaveStrategy()
    {
        if (saveStrat == null)
            saveStrat = new ProjectSaveStrategy();

        return saveStrat == null;
    }

    public void LoadUI(StackPanel mainPanel, GumService gum)
    {
        foreach (var projectFeature in projectFeatures)
        {
            projectFeature.LoadUI(mainPanel);
        }
        
        inProjectStack = new StackPanel
        {
            IsVisible = false
        };
        filePathChanged += (newPath) => { inProjectStack.IsVisible = newPath != string.Empty; };
        mainPanel.AddChild(inProjectStack);
        
        foreach (var projectFeature in inProjectFeatures)
        {
            projectFeature.LoadUI(inProjectStack);
        }

        // Tools
        toolContainer = new ContainerRuntime
        {
            Width = gum.CanvasWidth,
            Height = gum.CanvasHeight,
            X = 0,
            Y = 0,
            Visible = false
        };
        filePathChanged += (newPath) => { toolContainer.Visible = newPath != string.Empty; };
        toolContainer.Anchor(Anchor.Center);
        toolContainer.AddToRoot();

        toolBar = new ToolBarFeature(gum, this);
        toolBar.LoadUI(toolContainer);
    }

    public void ResetData()
    {
        levels = new();
        entityDatas = new();
        currentLevel = 0;
    }
}