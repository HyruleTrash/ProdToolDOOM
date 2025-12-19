using Microsoft.Xna.Framework;
using ProdToolDOOM.ProjectFeatures.Tools;

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
        currentProject = new Project();
        cmdHistory = new CommandHistory();
        toolManager = new ToolManager();
    }
    
    protected override void LoadContent()
    {
        // _spriteBatch = new SpriteBatch(GraphicsDevice); TODO load icons here
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetShortcuts(BaseShortcuts.baseShortcuts);
    }
    
    protected override void LoadUI()
    {
        base.LoadUI();
        currentProject.LoadUI(topBarLeft, gum);
    }
    
    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        toolManager?.Update(mouseState, dt);
    }
}