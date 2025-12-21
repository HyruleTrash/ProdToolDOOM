using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Managers;
using Gum.Wireframe;
using MonoGameGum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.GueDeriving;
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

    public Mouse mouse = new();
    protected StackPanel topBarRight;
    protected StackPanel topBarLeft;
    
    private SpriteBatch spriteBatchIcons;
    private Texture2D closeIcon;
    private Texture2D minimizeIcon;
    private Texture2D maximizeIcon;

    public ShortcutManager shortcutManager = new ();

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
            dragComponent?.Update(windowSize);
        };
    }

    protected void SetShortcuts(ShortcutManager.ShortCut[] shortcuts) => shortcutManager.AddShortCuts(shortcuts);
    
    protected override void Initialize()
    {
        gum.Initialize(this);
        base.Initialize();
        
        var windowSize = new Vector2(gum.CanvasWidth, gum.CanvasHeight);
        resizeComponent = new Window.ResizeComponent(windowSize, graphics, Window);
        dragComponent = new Window.DragComponent(windowSize, Window);

        LoadUIContainers();
        LoadUI();
        FinalizeUI();
    }
    
    protected override void LoadContent()
    {
        spriteBatchIcons = new SpriteBatch(GraphicsDevice);
        
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
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(UIParams.canvasColor);
        gum.Draw();
        base.Draw(gameTime);
    }
    
    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            keyboardState.IsKeyDown(Keys.Escape))
            Exit();
        
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        gum.Update(gameTime);
        base.Update(gameTime);

        shortcutManager.Update(keyboardState, dt);
        
        if (IsFocused())
            CheckOnHover(dt);

        CheckScreenSizeChange();
        
        mouse.Update();
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
        // Convert the mouse position into Gum's centered coordinate system
        var canvasCenter = new Vector2(gum.CanvasWidth, gum.CanvasHeight) * 0.5f;
        var centeredPoint = new Vector2(point.x - canvasCenter.x, canvasCenter.y - point.y);

        // Window bounds in Gum's centered coordinate system
        var windowLeft = Window.ClientBounds.Width * 0.5f;
        var windowRight = -Window.ClientBounds.Width * 0.5f;
        var windowTop = -Window.ClientBounds.Height * 0.5f;
        var windowBottom = Window.ClientBounds.Height * 0.5f;

        bool insideWidth = centeredPoint.x >= windowRight && centeredPoint.x <= windowLeft;
        bool insideHeight = centeredPoint.y >= windowTop && centeredPoint.y <= windowBottom;

        return insideWidth && insideHeight;
    }

    public bool WasMouseClickConsumedByGum()
    {
        return gum.Cursor.WindowOver != null;
    }
    
    public bool IsFocused()
    {
        return ProdToolDOOM.Window.Helper.HasFocus(Window.Handle);
    }
}