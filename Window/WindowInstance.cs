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

    protected StackPanel topBarRight;
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
    public rightClickManager rightClickManager;

    protected WindowInstance()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        
        IsMouseVisible = true;
        Window.IsBorderless = true;
        Window.AllowUserResizing = true;

        onScreenSizeChange += windowSize =>
        {
            gum.CanvasWidth = windowSize.x;
            gum.CanvasHeight = windowSize.y;
            resizeComponent?.ResizeSelectionBoxData(windowSize);
            dragComponent?.UpdateSize(windowSize);
        };

        UpdateRegister = [];
        Mouse = new Mouse(this);
        UpdateRegister.Add(Mouse);
    }

    protected void SetShortcuts(ShortcutManager.ShortCut[] shortcuts) => shortcutManager.AddShortCuts(shortcuts);
    
    protected override void Initialize()
    {
        gum.Initialize(this);
        base.Initialize();
        
        var windowSize = new Vector2(gum.CanvasWidth, gum.CanvasHeight);
        resizeComponent = new Window.ResizeComponent(windowSize, graphics, Window);
        dragComponent = new Window.DragComponent(windowSize, Window);

        shortcutManager = new ShortcutManager();
        UpdateRegister.Add(shortcutManager);

        LoadUIContainers();
        LoadUI();
        FinalizeUI();
    }
    
    protected override void LoadContent()
    {
        closeIcon = Content.Load<Texture2D>("Icons/Cross");
        minimizeIcon = Content.Load<Texture2D>("Icons/Minimize");
        maximizeIcon = Content.Load<Texture2D>("Icons/Expand");
    }

    private void LoadUIContainers()
    {
        dragComponent.LoadUI();
        
        topBarRight = new StackPanel
        {
            Visual =
            {
                ChildrenLayout = Gum.Managers.ChildrenLayout.LeftToRightStack
            },
            X = gum.CanvasWidth - UIParams.borderPadding,
            Y = UIParams.borderPadding
        };
        topBarRight.Anchor(Anchor.TopRight);
        
        topBarLeft = new StackPanel
        {
            X = UIParams.borderPadding,
            Y = UIParams.borderPadding
        };
        topBarLeft.Anchor(Anchor.TopLeft);
    }
    
    protected virtual void LoadUI()
    {
        var exitButton = new Button
        {
            Text = "X",
            Width = UIParams.minBoxSize,
            Height = UIParams.minBoxSize,
        };
        UIParams.SetDefaultButton(exitButton);
        UIParams.AddIconToButton(exitButton, closeIcon);
        exitButton.Click += (_, _) => Exit();
        
        var minimizeButton = new Button
        {
            Text = "",
            Width = UIParams.minBoxSize,
            Height = UIParams.minBoxSize,
        };
        UIParams.SetDefaultButton(minimizeButton);
        UIParams.AddIconToButton(minimizeButton, minimizeIcon);
        minimizeButton.Click += (_, _) =>
        {
            var handle = Window.Handle;
            if (handle == IntPtr.Zero) return;
            ProdToolDOOM.Window.Helper.Minimize(handle);
        };
        
        var maximizeButton = new Button
        {
            Text = "",
            Width = UIParams.minBoxSize,
            Height = UIParams.minBoxSize,
        };
        UIParams.SetDefaultButton(maximizeButton);
        UIParams.AddIconToButton(maximizeButton, maximizeIcon);
        maximizeButton.Click += (_, _) =>
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
        topBarRight.AddChild(maximizeButton);
        topBarRight.AddChild(minimizeButton);
        topBarRight.AddChild(exitButton);
    }

    private void FinalizeUI()
    {
        dragComponent?.FinalizeUI();
        topBarRight.AddToRoot();
        topBarLeft.AddToRoot();
        
        rightClickManager = new();
        UpdateRegister.Add(rightClickManager);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(UIParams.canvasColor);
        gum.Draw();
        base.Draw(gameTime);
    }
    
    protected override void Update(GameTime gameTime)
    {
        KeyboardState = Keyboard.GetState();
        dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Mouse.currentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            KeyboardState.IsKeyDown(Keys.Escape))
            Exit();

        gum.Update(gameTime);
        base.Update(gameTime);

        updateRegister = UpdateRegister.ToList();
        foreach (var baseUpdatable in updateRegister)
        {
            if (baseUpdatable is null)
                continue;
            baseUpdatable.Update(dt, this);
        }
        
        if (IsFocused())
            CheckOnHover(dt);

        CheckScreenSizeChange();
        Mouse.UpdateVisual();
    }

    private void CheckScreenSizeChange()
    {
        if (!shouldCallOnScreenSizeChanged) return;
        const float tolerance = 0.1f;
        if (Math.Abs(Window.ClientBounds.Width - gum.CanvasWidth) < tolerance && Math.Abs(Window.ClientBounds.Height - gum.CanvasHeight) < tolerance) return;
        onScreenSizeChange?.Invoke(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));
        shouldCallOnScreenSizeChanged = false;
    }

    private void CheckOnHover(float dt)
    {
        var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        
        // TODO Check any other hover elements
        
        if (Fullscreen)
            return;
        var dragging = dragComponent?.CheckHover(mouseState, dt);
        if (dragging is null or false)
        {
            resizeComponent?.CheckHover(mouseState, dt);
            resizeComponent?.ResizeWindow();
        }
    }
    
    public GameWindow GetWindow() => Window;
    public bool IsInsideWindowBounds(Vector2 point)
    {
        var width = Window.ClientBounds.Width - UIParams.minNearSelection;
        var height = Window.ClientBounds.Height - UIParams.minNearSelection;
        
        // Convert the mouse position into Gum's centered coordinate system
        var canvasCenter = new Vector2(gum.CanvasWidth, gum.CanvasHeight) * 0.5f;
        var centeredPoint = new Vector2(point.x - canvasCenter.x, canvasCenter.y - point.y);

        // Window bounds in Gum's centered coordinate system
        var windowLeft = width * 0.5f;
        var windowRight = -width * 0.5f;
        var windowTop = -height * 0.5f;
        var windowBottom = height * 0.5f;

        bool insideWidth = centeredPoint.x >= windowRight && centeredPoint.x <= windowLeft;
        bool insideHeight = centeredPoint.y >= windowTop && centeredPoint.y <= windowBottom;

        return insideWidth && insideHeight;
    }

    public bool WasMouseClickConsumedByGum() => gum.Cursor.WindowOver != null;

    public bool IsFocused() => ProdToolDOOM.Window.Helper.HasFocus(Window.Handle);

    public Vector2 GetWindowSize()
    {
        return new Vector2(GetWindowWidth(), GetWindowHeight());
    }
    
    public float GetWindowWidth() => Window.ClientBounds.Width;
    public float GetWindowHeight() => Window.ClientBounds.Height;
}