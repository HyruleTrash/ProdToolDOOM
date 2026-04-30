using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Managers;
using Gum.Wireframe;
using MonoGameGum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.GueDeriving;
using ProdToolDOOM.ProjectFeatures;
using Button = Gum.Forms.Controls.Button;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ProdToolDOOM;

public class WindowInstance : Game
{
    protected GraphicsDeviceManager graphics;
    protected GumService gum => GumService.Default;
    
    public Action<Vector2>? onScreenSizeChange;
    public bool Fullscreen { get; private set; }

    private bool shouldCallOnScreenSizeChanged;
    private Window.ResizeComponent? resizeComponent;
    private Window.DragComponent? dragComponent;

    public StackPanel TopBarRight { get; private set; }
    protected StackPanel topBarLeft;
    
    private Texture2D closeIcon;
    private Texture2D minimizeIcon;
    private Texture2D maximizeIcon;

    public List<IBaseUpdatable> UpdateRegister { get; set; }
    private List<IBaseUpdatable> updateRegister = [];
    public KeyboardState KeyboardState { get; private set; }
    protected float dt;
    public Mouse Mouse { get; private set; }
    
    public ShortcutManager shortcutManager;
    public RightClickManager rightClickManager;

    protected WindowInstance()
    {
        this.graphics = new GraphicsDeviceManager(this);
        this.Content.RootDirectory = "Content";

        this.IsMouseVisible = true;
        this.Window.IsBorderless = true;
        this.Window.AllowUserResizing = true;

        this.onScreenSizeChange += windowSize =>
        {
            this.gum.CanvasWidth = windowSize.x;
            this.gum.CanvasHeight = windowSize.y;
            this.resizeComponent?.ResizeSelectionBoxData(windowSize);
            this.dragComponent?.UpdateSize(windowSize);
        };

        this.UpdateRegister = [];
        this.Mouse = new Mouse(this);
        this.UpdateRegister.Add(this.Mouse);
    }

    protected void SetShortcuts(ShortcutManager.ShortCut[] shortcuts) => this.shortcutManager.AddShortCuts(shortcuts);
    
    protected override void Initialize()
    {
        this.gum.Initialize(this);
        base.Initialize();
        
        Vector2 windowSize = new(this.gum.CanvasWidth, this.gum.CanvasHeight);
        this.resizeComponent = new Window.ResizeComponent(windowSize, this.graphics, this.Window);
        this.dragComponent = new Window.DragComponent(windowSize, this.Window);

        this.shortcutManager = new ShortcutManager();
        this.UpdateRegister.Add(this.shortcutManager);

        LoadUIContainers();
        LoadUI();
        FinalizeUI();
    }
    
    protected override void LoadContent()
    {
        this.closeIcon = this.Content.Load<Texture2D>("Icons/Cross");
        this.minimizeIcon = this.Content.Load<Texture2D>("Icons/Minimize");
        this.maximizeIcon = this.Content.Load<Texture2D>("Icons/Expand");
    }

    private void LoadUIContainers()
    {
        this.dragComponent?.LoadUI();

        this.TopBarRight = new StackPanel
        {
            Visual =
            {
                ChildrenLayout = ChildrenLayout.LeftToRightStack
            },
            X = this.gum.CanvasWidth - UIParams.borderPadding,
            Y = UIParams.borderPadding
        };
        this.TopBarRight.Anchor(Anchor.TopRight);

        this.topBarLeft = new StackPanel
        {
            X = UIParams.borderPadding,
            Y = UIParams.borderPadding
        };
        this.topBarLeft.Anchor(Anchor.TopLeft);
    }
    
    protected virtual void LoadUI()
    {
        Button exitButton = new()
        {
            Text = "X",
            Width = UIParams.minBoxSize,
            Height = UIParams.minBoxSize,
        };
        UIParams.SetDefaultButton(exitButton);
        UIParams.AddIconToButton(exitButton, this.closeIcon);
        exitButton.Click += (_, _) => Exit();
        
        Button minimizeButton = new()
        {
            Text = "",
            Width = UIParams.minBoxSize,
            Height = UIParams.minBoxSize,
        };
        UIParams.SetDefaultButton(minimizeButton);
        UIParams.AddIconToButton(minimizeButton, this.minimizeIcon);
        minimizeButton.Click += (_, _) =>
        {
            IntPtr handle = this.Window.Handle;
            if (handle == IntPtr.Zero) return;
            ProdToolDOOM.Window.Helper.Minimize(handle);
        };
        
        Button maximizeButton = new()
        {
            Text = "",
            Width = UIParams.minBoxSize,
            Height = UIParams.minBoxSize,
        };
        UIParams.SetDefaultButton(maximizeButton);
        UIParams.AddIconToButton(maximizeButton, this.maximizeIcon);
        maximizeButton.Click += (_, _) =>
        {
            IntPtr handle = this.Window.Handle;
            if (handle == IntPtr.Zero) return;

            if (this.Fullscreen)
            {
                ProdToolDOOM.Window.Helper.UnMaximize(handle);
                this.Fullscreen = false;
            }
            else
            {
                ProdToolDOOM.Window.Helper.Maximize(handle);
                this.Fullscreen = true;
            }

            this.shouldCallOnScreenSizeChanged = true;
        };
        this.TopBarRight.AddChild(maximizeButton);
        this.TopBarRight.AddChild(minimizeButton);
        this.TopBarRight.AddChild(exitButton);
    }

    private void FinalizeUI()
    {
        this.dragComponent?.FinalizeUI();
        this.TopBarRight.AddToRoot();
        this.topBarLeft.AddToRoot();

        this.rightClickManager = new RightClickManager();
        this.UpdateRegister.Add(this.rightClickManager);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        this.GraphicsDevice.Clear(UIParams.canvasColor);
        this.gum.Draw();
        base.Draw(gameTime);
    }
    
    protected override void Update(GameTime gameTime)
    {
        this.KeyboardState = Keyboard.GetState();
        this.dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        this.Mouse.currentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || this.KeyboardState.IsKeyDown(Keys.Escape))
            Exit();

        this.gum.Update(gameTime);
        base.Update(gameTime);

        this.updateRegister = this.UpdateRegister.ToList();
        foreach (var baseUpdatable in this.updateRegister)
        {
            if (baseUpdatable is null)
                continue;
            baseUpdatable.Update(this.dt, this);
        }
        
        if (IsFocused())
            CheckOnHover(this.dt);

        CheckScreenSizeChange();
        this.Mouse.UpdateVisual();
    }

    private void CheckScreenSizeChange()
    {
        if (!this.shouldCallOnScreenSizeChanged) return;
        const float tolerance = 0.1f;
        if (Math.Abs(this.Window.ClientBounds.Width - this.gum.CanvasWidth) < tolerance && Math.Abs(this.Window.ClientBounds.Height - this.gum.CanvasHeight) < tolerance) return;
        this.onScreenSizeChange?.Invoke(new Vector2(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height));
        this.shouldCallOnScreenSizeChanged = false;
    }

    private void CheckOnHover(float dt)
    {
        MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        
        // TODO Check any other hover elements
        
        if (this.Fullscreen)
            return;
        bool? dragging = this.dragComponent?.CheckHover(mouseState, dt);
        if (dragging is null or false)
        {
            this.resizeComponent?.CheckHover(mouseState, dt);
            this.resizeComponent?.ResizeWindow();
        }
    }
    
    public GameWindow GetWindow() => this.Window;
    public bool IsInsideWindowBounds(Vector2 point)
    {
        float width = this.Window.ClientBounds.Width - UIParams.minNearSelection;
        float height = this.Window.ClientBounds.Height - UIParams.minNearSelection;
        
        // Convert the mouse position into Gum's centered coordinate system
        Vector2 canvasCenter = new Vector2(this.gum.CanvasWidth, this.gum.CanvasHeight) * 0.5f;
        Vector2 centeredPoint = new(point.x - canvasCenter.x, canvasCenter.y - point.y);

        // Window bounds in Gum's centered coordinate system
        float windowLeft = width * 0.5f;
        float windowRight = -width * 0.5f;
        float windowTop = -height * 0.5f;
        float windowBottom = height * 0.5f;

        bool insideWidth = centeredPoint.x >= windowRight && centeredPoint.x <= windowLeft;
        bool insideHeight = centeredPoint.y >= windowTop && centeredPoint.y <= windowBottom;

        return insideWidth && insideHeight;
    }

    public bool WasMouseClickConsumedByGum() => this.gum.Cursor.WindowOver != null;

    public bool IsFocused() => ProdToolDOOM.Window.Helper.HasFocus(this.Window.Handle);

    public Vector2 GetWindowSize()
    {
        return new Vector2(GetWindowWidth(), GetWindowHeight());
    }
    
    public float GetWindowWidth() => this.Window.ClientBounds.Width;
    public float GetWindowHeight() => this.Window.ClientBounds.Height;
}