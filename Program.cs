using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using ProdToolDOOM.Window;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Program : Game
{
    public static Program instance;
    public readonly string PROGRAM_VERSION = "0.0.1";
    
    private GraphicsDeviceManager graphics;
    private GumService gum => GumService.Default;
    
    public readonly Action<Vector2>? onScreenSizeChange;
    public bool Fullscreen { get => fullscreen; private set => fullscreen = value; }
    private bool shouldCallOnScreenSizeChanged;
    private bool fullscreen;
    private ResizeManager? resizeManager;
    
    private List<MouseVisualSetCall> mouseVisualSetCalls = [];

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
        
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        
        IsMouseVisible = true;
        Window.IsBorderless = true;
        Window.AllowUserResizing = true;

        onScreenSizeChange += size =>
        {
            gum.CanvasWidth = size.x;
            gum.CanvasHeight = size.y;
            resizeManager?.ResizeSelectionBoxData(size);
        };
    }
    
    protected override void Initialize()
    {
        gum.Initialize(this);
        LoadUI();
        base.Initialize();
        
        resizeManager = new ResizeManager(new Vector2(gum.CanvasWidth, gum.CanvasHeight));
    }
    
    protected override void LoadContent()
    {
        // _spriteBatch = new SpriteBatch(GraphicsDevice); TODO load icons here
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
            ProdToolDOOM.Window.Helper.Minimize(handle);
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
                ProdToolDOOM.Window.Helper.UnMaximize(handle);
                Fullscreen = false;
            }
            else
            {
                ProdToolDOOM.Window.Helper.Maximize(handle);
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
        
        currentProject.LoadUI(mainPanel, gum);
    }

    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        gum.Draw();
        base.Draw(gameTime);
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        gum.Update(gameTime);
        base.Update(gameTime);
        
        if (ProdToolDOOM.Window.Helper.HasFocus(Window.Handle))
            CheckOnHover();
        resizeManager?.ResizeWindow(graphics, Window);

        CheckScreenSizeChange();
        
        // Update mouse visual
        var mouseVisual = GetMouseVisual();
        if (mouseVisual != null)
            Mouse.SetCursor(mouseVisual);
        mouseVisualSetCalls = [new MouseVisualSetCall(MouseCursor.Arrow, 0)];
    }

    private void CheckScreenSizeChange()
    {
        if (!shouldCallOnScreenSizeChanged) return;
        const float tolerance = 0.1f;
        if (Math.Abs(Window.ClientBounds.Width - gum.CanvasWidth) < tolerance && Math.Abs(Window.ClientBounds.Height - gum.CanvasHeight) < tolerance) return;
        onScreenSizeChange?.Invoke(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));
        shouldCallOnScreenSizeChanged = false;
    }

    private void CheckOnHover()
    {
        var mouseState = Mouse.GetState();
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);
        
        if (Fullscreen)
            return;
        resizeManager?.CheckResizePositions(mousePosition, mouseState);
    }

    public void SetMouseVisual(MouseCursor visualType, int priority)
    {
        mouseVisualSetCalls.Add(new MouseVisualSetCall(visualType, priority));
    }

    private MouseCursor? GetMouseVisual()
    {
        MouseVisualSetCall? currentHighestPriorityCall = null;
        foreach (var mouseVisualSetCall in mouseVisualSetCalls)
        {
            if (currentHighestPriorityCall == null)
            {
                currentHighestPriorityCall = mouseVisualSetCall;
                continue;
            }
            if (currentHighestPriorityCall.Value.priority < mouseVisualSetCall.priority)
            {
                currentHighestPriorityCall = mouseVisualSetCall;
            }
        }

        return currentHighestPriorityCall?.type;
    }
}