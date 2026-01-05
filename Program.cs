using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProdToolDOOM.ProjectFeatures;
using ProdToolDOOM.ProjectFeatures.Tools;
using Color = Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM;

public class Program : WindowInstance
{
    public static Program instance = null!;
    public readonly string PROGRAM_VERSION = "0.0.2";

    public Project currentProject;
    public CommandHistory cmdHistory;
    private int currentLevel;
    public ToolManager? toolManager;

    [STAThread]
    static void Main(string[] _)
    {
        Debug.Log("Starting application...");
        var p = new Program();
        p.Run();
    }
    
    private Program()
    {
        instance = this;
        currentProject = new Project(gum);
        cmdHistory = new CommandHistory();
    }

    protected override void Initialize()
    {
        base.Initialize();
        toolManager = new ToolManager(this);
        UpdateRegister.Add(toolManager);
        SetShortcuts(BaseShortcuts.baseShortcuts);
        RightClickRegister.Register(rightClickManager);
    }
    
    protected override void LoadUI()
    {
        currentProject.LoadUI(topBarLeft);
        base.LoadUI();
    }
}