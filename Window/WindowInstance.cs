using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Wireframe;
using MonoGameGum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Button = Gum.Forms.Controls.Button;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ProdToolDOOM;

public class WindowInstance : Game
{
    protected GraphicsDeviceManager graphics;
    protected GumService gum => GumService.Default;
    
    public readonly Action<Vector2>? onScreenSizeChange;
    public bool Fullscreen { get; private set; }

    private bool shouldCallOnScreenSizeChanged;
    private Window.ResizeComponent? resizeComponent;
    private Window.DragComponent? dragComponent;

    public Mouse mouse = new();
    protected StackPanel topBarRight;
    protected StackPanel topBarLeft;

    public ShortcutManager shortcutManager = new ();

    protected WindowInstance()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        
        IsMouseVisible = true;
        Window.IsBorderless = true;
        Window.AllowUserResizing = true;

        onScreenSizeChange += size =>
        {
            gum.CanvasWidth = size.x;
            gum.CanvasHeight = size.y;
            resizeComponent?.ResizeSelectionBoxData(size);
            dragComponent?.Update(size);
        };
    }

    public void SetShortcuts(ShortcutManager.ShortCut[] shortcuts) => shortcutManager.AddShortCuts(shortcuts);
    
    protected override void Initialize()
    {
        gum.Initialize(this);
        base.Initialize();
        
        var windowSize = new Vector2(gum.CanvasWidth, gum.CanvasHeight);
        resizeComponent = new Window.ResizeComponent(windowSize, graphics, Window);
        dragComponent = new Window.DragComponent(windowSize, Window);
        
        LoadUI();
    }
    
    protected virtual void LoadUI()
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
        topBarRight.AddToRoot();
        
        var exitButton = new Button
        {
            Text = "X",
            Width = UIParams.minBoxSize,
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton((ButtonVisual)exitButton.Visual);
        exitButton.Click += (sender, args) => Exit();
        
        var minimizeButton = new Button
        {
            Text = "_",
            Width = UIParams.minBoxSize,
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton((ButtonVisual)minimizeButton.Visual);
        minimizeButton.Click += (sender, args) =>
        {
            var handle = Window.Handle;
            if (handle == IntPtr.Zero) return;
            ProdToolDOOM.Window.Helper.Minimize(handle);
        };
        
        var maximizeButton = new Button
        {
            Text = "[ ]",
            Width = UIParams.minBoxSize
        };
        UIParams.SetDefaultButton((ButtonVisual)maximizeButton.Visual);
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
        topBarRight.AddChild(maximizeButton);
        topBarRight.AddChild(minimizeButton);
        topBarRight.AddChild(exitButton);
        
        topBarLeft = new StackPanel
        {
            X = UIParams.borderPadding,
            Y = UIParams.borderPadding
        };
        topBarLeft.Anchor(Anchor.TopLeft);
        topBarLeft.AddToRoot();
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
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
        
        if (ProdToolDOOM.Window.Helper.HasFocus(Window.Handle))
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
}