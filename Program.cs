using Gum.Forms.Controls;
using Gum.Wireframe;
using Button = Gum.Forms.Controls.Button;
using MonoGameGum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProdToolDOOM.Window;

namespace ProdToolDOOM;

public class Program : WindowInstance
{
    public static Program instance = null!;
    public readonly string PROGRAM_VERSION = "0.0.1";

    public Project currentProject;
    public CommandHistory cmdHistory;
    private int currentLevel;
    
    [STAThread]
    static void Main(string[] args)
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
    }
}