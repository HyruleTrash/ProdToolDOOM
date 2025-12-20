using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    
    // TODO REMOVE TEMP
    private SpriteBatch spriteBatch;
    private Texture2D pointIcon;

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
        spriteBatch = new SpriteBatch(GraphicsDevice);
        
        pointIcon = Content.Load<Texture2D>("Icons/Point");
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

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        spriteBatch.Begin();
        spriteBatch.Draw(pointIcon, Microsoft.Xna.Framework.Vector2.Zero, null, Color.White, 0.0f, Microsoft.Xna.Framework.Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        spriteBatch.End();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        toolManager?.Update(mouseState, dt);
    }
}