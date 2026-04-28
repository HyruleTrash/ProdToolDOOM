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
        Program p = new Program();
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
        this.toolManager = new ToolManager(this);
        this.UpdateRegister.Add(this.toolManager);
        SetShortcuts(BaseShortcuts.baseShortcuts);
        RightClickRegister.Register(this.rightClickManager);
    }
    
    protected override void LoadUI()
    {
        this.currentProject.LoadUI(this.topBarLeft);
        base.LoadUI();
    }
}