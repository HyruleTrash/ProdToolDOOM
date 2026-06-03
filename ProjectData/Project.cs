

using System;
using System.Collections.Generic;
using System.Linq;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using DLLevelBuilder.ProjectFeatures;
using DLLevelBuilder.ProjectFeatures.Exporting;
using DLLevelBuilder.Version2;
using Gum.Forms.Controls;

namespace DLLevelBuilder;

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
    public IReadOnlyDictionary<int, EntityData> EntityDatas => this.entityDatas;
    private Dictionary<int, EntityData> entityDatas = [];
    public int entityDataIdCounter = 0;
    public Action<IReadOnlyDictionary<int, EntityData>> onEntityDataChanged;
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
    
    private readonly LoadFeature loadFeature;
    private readonly SaveNewFeature saveNewFeature;
    private readonly SaveFeature saveFeature;
    private readonly ExportFeature exportFeature;
    private readonly SwitchLevelFeature switchLevelFeature; // TODO load ui
    private readonly EntityDataManageFeature entityDataManagerFeature; // TODO load ui
    private readonly ToolBarFeature toolBar;
    
    // UI
    private MenuItem projectMenuItem;
    private MenuItem levelMenuItem;
    public ContainerRuntime ToolContainer { get; private set; }
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
        this.entityDataManagerFeature = new EntityDataManageFeature(this);
        this.toolBar = new ToolBarFeature(gum, this);
    }

    public static Level TryGetCurrentLevel()
    {
        if (instance.currentLevel < 0 || instance.currentLevel >= instance.levels.Count) return null;
        return instance.levels[instance.currentLevel];
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
        
        this.toolBar.LoadUI(this.ToolContainer);
        
        TopLeftUI(topBarLeft);
        TopRightUI(topBarRight);
    }

    private void TopLeftUI(Menu topLeftMenu)
    {
        this.projectMenuItem = new MenuItem { Header = "Project" };
        topLeftMenu.Items.Add(this.projectMenuItem);
        this.levelMenuItem = new MenuItem
        {
            Header = "Level",
            IsVisible = false
        };

        this.loadFeature.LoadUI(this.projectMenuItem, true);
        this.saveNewFeature.LoadUI(this.projectMenuItem, true);
        
        this.saveFeature.LoadUI(this.projectMenuItem, false);
        this.exportFeature.LoadUI(this.levelMenuItem, false);
        this.switchLevelFeature.LoadUI(this.levelMenuItem, false);
        
        this.filePathChanged += (newPath) =>
        {
            bool state = newPath != string.Empty;
            this.saveFeature.SetVisible(state);
            this.exportFeature.SetVisible(state);
            this.switchLevelFeature.SetVisible(state);
            
            this.levelMenuItem.IsVisible = state;
            if (state)
            {
                if (topLeftMenu.Items.Contains(this.levelMenuItem))
                    return;
                topLeftMenu.Items.Add(this.levelMenuItem);
            }
            else
            {
                if (!topLeftMenu.Items.Contains(this.levelMenuItem))
                    return;
                topLeftMenu.Items.Remove(this.levelMenuItem);
            }
        };
    }

    private void TopRightUI(Menu topBarRight)
    {
        // throw new NotImplementedException();
    }

    public void ResetData()
    {
        this.levels = new List<Level>();
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
}