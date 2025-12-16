using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

class Program : Game
{
    public static string PROGRAM_VERSION = "0.0.1";
    public Action<Vector2>? onScreenSizeChange = null;
    private bool shouldCallOnScreenSizeChanged;
    public bool Fullscreen { get => fullscreen; private set => fullscreen = value; }
    private bool fullscreen = false;
    private GraphicsDeviceManager graphics;
    private GumService gum => GumService.Default;
    
    public CommandHistory cmdHistory;
    private int currentLevel;
    
    public Program()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.IsBorderless = true;
        Window.AllowUserResizing = true;

        onScreenSizeChange += size =>
        {
            Debug.Log("Screen size change: " + gum.CanvasWidth  + ", " + gum.CanvasHeight);
            Debug.Log("Screen size change: " + size);
            gum.CanvasWidth = size.x;
            gum.CanvasHeight = size.y;
        };
    }
    
    [STAThread]
    static void Main(string[] args)
    {
        Debug.Log("Starting application...");
        var p = new Program();
        p.Run();
    }
    
    protected override void Initialize()
    {
        gum.Initialize(this);
        LoadUI();
        base.Initialize();
    }

    private void LoadUI()
    {
        var exitPanel = new StackPanel
        {
            Visual =
            {
                ChildrenLayout = Gum.Managers.ChildrenLayout.LeftToRightStack
            },
            X = gum.CanvasWidth - UIParams.borderPadding,
            Y = UIParams.borderPadding
        };
        exitPanel.Anchor(Anchor.TopRight);
        exitPanel.AddToRoot();
        
        var exitButton = new Button
        {
            Text = "X",
            Width = UIParams.minBoxSizeAroundText
        };
        exitButton.Click += (sender, args) => Exit();
        
        var minimizeButton = new Button
        {
            Text = "_",
            Width = UIParams.minBoxSizeAroundText
        };
        minimizeButton.Click += (sender, args) =>
        {
            var handle = Window.Handle;
            if (handle == IntPtr.Zero) return;
            WindowHelper.Minimize(handle);
        };
        
        var maximizeButton = new Button
        {
            Text = "[ ]",
            Width = UIParams.minBoxSizeAroundText
        };
        maximizeButton.Click += (sender, args) =>
        {
            var handle = Window.Handle;
            if (handle == IntPtr.Zero) return;

            if (Fullscreen)
            {
                WindowHelper.UnMaximize(handle);
                Fullscreen = false;
            }
            else
            {
                WindowHelper.Maximize(handle);
                Fullscreen = true;
            }

            shouldCallOnScreenSizeChanged = true;
        };
        exitPanel.AddChild(maximizeButton);
        exitPanel.AddChild(minimizeButton);
        exitPanel.AddChild(exitButton);
        
        var mainPanel = new StackPanel
        {
            X = UIParams.borderPadding,
            Y = UIParams.borderPadding
        };
        mainPanel.Anchor(Anchor.TopLeft);
        mainPanel.AddToRoot();
        
        Project.LoadUI(mainPanel, gum);
    }
    
    protected override void LoadContent()
    {
        // _spriteBatch = new SpriteBatch(GraphicsDevice); TODO load icons here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        gum.Update(gameTime);
        base.Update(gameTime);

        CheckScreenSizeChange();
    }

    private void CheckScreenSizeChange()
    {
        if (!shouldCallOnScreenSizeChanged) return;
        const float tolerance = 0.1f;
        if (Math.Abs(Window.ClientBounds.Width - gum.CanvasWidth) < tolerance && Math.Abs(Window.ClientBounds.Height - gum.CanvasHeight) < tolerance) return;
        onScreenSizeChange?.Invoke(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));
        shouldCallOnScreenSizeChanged = false;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        gum.Draw();
        base.Draw(gameTime);
    }
}