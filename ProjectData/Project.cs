
using System;
using System.Collections.Generic;
using System.Linq;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using DLLevelBuilder.ProjectFeatures;
using DLLevelBuilder.ProjectFeatures.Exporting;
using DLLevelBuilder.UI;
using DLLevelBuilder.Version3;
using Gum.Forms.Controls;

namespace DLLevelBuilder;

public class Project
{
    public static Project Instance => Program.instance.currentProject;
    // file reading
    private IProjectSaveStrategy? saveStrat;
    private IProjectLoadStrategy? loadStrat;
    public string FilePath
    {
        get => this.filePath;
        private set
        {
            if (this.filePath != value) this.filePathChanged.Invoke(value);
            this.filePath = value;
        }
    }
    private string filePath = string.Empty;
    public Action<string> filePathChanged;
    // data
    public IReadOnlyDictionary<int, EntityData> EntityDatas => this.entityDatas;
    private Dictionary<int, EntityData> entityDatas = [];
    public int entityDataIdCounter = 0;
    private Action<IReadOnlyDictionary<int, EntityData>> onEntityDataChanged = null!;
    public Dictionary<int, Level> levels = [];
    private Action<IReadOnlyDictionary<int, Level>> onLevelsChanged = null!;

    public int CurrentLevel
    {
        get => this.currentLevel;
        set
        {
            if (!this.levels.TryGetValue(value, out Level? level)) return;
            if (this.currentLevel != value)
            {
                level.CheckInit();
                this.onCurrentLevelChanged?.Invoke(value);
            }
            this.currentLevel = value;
        }
    }
    private int currentLevel = -1;
    public Action<int>? onCurrentLevelChanged;
    
    private readonly LoadFeature loadFeature;
    private readonly SaveNewFeature saveNewFeature;
    private readonly SaveFeature saveFeature;
    private readonly ExportFeature exportFeature;
    private readonly SwitchLevelFeature switchLevelFeature;
    private readonly OpenLevelManagerFeature openLevelManagerFeature;
    private readonly NewLevelFeature newLevelFeature;
    private readonly OpenEntityDataManageFeature openEntityDataManagerFeature;
    private readonly NewEntityDataFeature newEntityDataFeature;
    private readonly ToolBarFeature toolBar;
    
    // UI
    private MenuItem projectMenuItem = null!;
    private MenuItem levelMenuItem = null!;
    private MenuItem entityMenuItem = null!;
    private readonly List<MenuItem> topBarRightMenuItems = [];
    private ContainerRuntime ToolContainer { get; set; } = null!;
    public ContainerRuntime canvasContainer = null!;
    public ContainerRuntime popUpContainer = null!;
    private readonly GumService gum;

    public Project(GumService gum)
    {
        this.gum = gum;
        this.filePathChanged = newPath => { Debug.Log($"FilePathChanged: {newPath}"); };

        this.loadFeature = new LoadFeature(this);
        this.saveNewFeature = new SaveNewFeature(this);
        this.saveFeature = new SaveFeature(this);
        this.exportFeature = new ExportFeature(this);
        this.switchLevelFeature = new SwitchLevelFeature(this);
        this.openLevelManagerFeature = new OpenLevelManagerFeature();
        this.newLevelFeature = new NewLevelFeature(this);
        this.openEntityDataManagerFeature = new OpenEntityDataManageFeature();
        this.newEntityDataFeature = new NewEntityDataFeature();
        this.toolBar = new ToolBarFeature(gum);
    }

    public static Level? TryGetCurrentLevel() => Instance.levels.GetValueOrDefault(Instance.currentLevel);
    public void SetLevelNearestId()
    {
        bool result = CheckAndMakeRequiredLevel();
        this.CurrentLevel = this.levels.Keys.OrderBy(k => Math.Abs(k - this.currentLevel)).FirstOrDefault();
        if (result && this.currentLevel == 0) this.onCurrentLevelChanged?.Invoke(0);
    }

    private bool CheckAndMakeRequiredLevel()
    {
        if (this.levels.Count != 0) return false;
        AddLevelCmd cmd = new(this);
        cmd.Execute();
        return true;
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
    
    public void Save(string tempPath)
    {
        if (this.saveStrat != null && this.saveStrat.Save(tempPath)) this.FilePath = tempPath;
        CheckAndMakeRequiredLevel();
    }
    
    public void Load(string tempPath)
    {
        if (this.loadStrat != null && this.loadStrat.Load(tempPath)) this.FilePath = tempPath;
        CheckAndMakeRequiredLevel();
        this.onEntityDataChanged?.Invoke(this.entityDatas);
        this.onLevelsChanged?.Invoke(this.levels);
    }

    public void LoadUI(Menu topBarLeft, Menu topBarRight)
    {
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
        this.filePathChanged += newPath => { this.ToolContainer.Visible = newPath != string.Empty; };
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
        
        this.toolBar.LoadUI(this.ToolContainer);
        
        TopLeftUI(topBarLeft);
        TopRightUI(topBarRight);
        
        this.filePathChanged += _ => Program.instance.UpdateTopBars();
    }

    private void TopLeftUI(Menu topLeftMenu)
    {
        this.projectMenuItem = new MenuItem { Header = "Project" };
        CustomMenuItemVisual.Create(this.projectMenuItem);
        topLeftMenu.Items.Add(this.projectMenuItem);

        this.loadFeature.LoadUI(this.projectMenuItem);
        this.saveNewFeature.LoadUI(this.projectMenuItem);
        this.saveFeature.LoadUI(this.projectMenuItem, false);
        
        this.filePathChanged += newPath =>
        {
            bool state = newPath != string.Empty;
            this.saveFeature.SetVisible(state);
        };
    }

    private void TopRightUI(Menu topBarRight)
    {
        this.levelMenuItem = new MenuItem
        {
            Header = "Level",
            IsVisible = false
        };
        topBarRight.Items.Add(this.levelMenuItem);
        CustomMenuItemVisual.Create(this.levelMenuItem);
        this.entityMenuItem = new MenuItem
        {
            Header = "Entity",
            IsVisible = false
        };
        topBarRight.Items.Add(this.entityMenuItem);
        CustomMenuItemVisual.Create(this.entityMenuItem);
        
        this.newLevelFeature.LoadUI(this.levelMenuItem);
        this.openLevelManagerFeature.LoadUI(this.levelMenuItem);
        this.exportFeature.LoadUI(this.levelMenuItem);
        this.switchLevelFeature.LoadUI(this.levelMenuItem);
        
        this.openEntityDataManagerFeature.LoadUI(this.entityMenuItem);
        this.newEntityDataFeature.LoadUI(this.entityMenuItem);
        
        this.topBarRightMenuItems.Add(this.levelMenuItem);
        this.topBarRightMenuItems.Add(this.entityMenuItem);
        
        this.filePathChanged += newPath =>
        {
            bool state = newPath != string.Empty;
            this.topBarRightMenuItems.SetVisibility(topBarRight, state);
        };
    }

    public void ResetData()
    {
        this.levels = new Dictionary<int, Level>();
        this.entityDatas = new Dictionary<int, EntityData>();
        this.CurrentLevel = -1;
        Program.instance.cmdHistory.Reset();
        this.canvasContainer.Children?.Clear();
    }

    public void AddEntityData(int id, EntityData entityData, bool silent = false)
    {
        this.entityDatas.Add(id, entityData);
        if (silent) return;
        this.onEntityDataChanged?.Invoke(this.EntityDatas);
    }
    
    public void RemoveEntityData(int id)
    {
        this.entityDatas.Remove(id);
        this.onEntityDataChanged?.Invoke(this.EntityDatas);
    }

    public string? TryGetEntityName(int dataId) => 
        (from keyValuePair in this.entityDatas where keyValuePair.Key == dataId select keyValuePair.Value.Name).FirstOrDefault();

    public void AddLevel(Level level)
    {
        this.levels.Add(level.LevelId, level);
        this.onLevelsChanged?.Invoke(this.levels);
        if (level.LevelId == this.currentLevel) this.onCurrentLevelChanged?.Invoke(this.currentLevel);
    }
    
    public void RemoveLevel(Level level)
    {
        this.levels.Remove(level.LevelId);
        this.onLevelsChanged?.Invoke(this.levels);
    }

    public int GetLowestUnusedLevelId()
    {
        int counter = 0;
        while (true)
        {
            if (!this.levels.ContainsKey(counter)) return counter;
            counter++;
        }
    }

    public void RegisterOnLevelsChanged(Action<IReadOnlyDictionary<int, Level>> listener) => this.onLevelsChanged += listener;
    public void RegisterOnEntityDataChanged(Action<IReadOnlyDictionary<int, EntityData>> listener) => this.onEntityDataChanged += listener;
}