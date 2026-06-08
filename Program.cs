using System;
using DLLevelBuilder.ProjectFeatures;
using DLLevelBuilder.ProjectFeatures.Tools;

namespace DLLevelBuilder;

public class Program : WindowInstance
{
    public static Program instance = null!;
    public readonly string PROGRAM_VERSION = "0.0.2";

    public Project currentProject;
    public CommandHistory cmdHistory;
    private int currentLevel;

    [STAThread]
    static void Main(string[] _)
    {
        Debug.Log("Starting application...");
        Program p = new();
        p.Run();
    }
    
    private Program()
    {
        instance = this;
        this.currentProject = new Project(this.gum);
        this.cmdHistory = new CommandHistory();
    }

    protected override void Initialize()
    {
        base.Initialize();
        this.UpdateRegister.Add(ToolManager.Instance);
        SetShortcuts(BaseShortcuts.baseShortcuts);
        RightClickRegister.Register(this.rightClickManager);
    }
    
    protected override void LoadUI()
    {
        this.currentProject.LoadUI(this.topBarLeft, this.TopBarRight);
        base.LoadUI();
    }
}