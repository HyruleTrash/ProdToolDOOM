using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using ProdToolDOOM.Window;
using Button = Gum.Forms.Controls.Button;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ProdToolDOOM;

public class WindowInstance : Game
{
    protected GraphicsDeviceManager graphics;
    protected GumService gum => GumService.Default;
    
    public readonly Action<Vector2>? onScreenSizeChange;
    public bool Fullscreen { get => fullscreen; private set => fullscreen = value; }
    private bool shouldCallOnScreenSizeChanged;
    private bool fullscreen;
    private ResizeManager? resizeManager;

    public Mouse mouse = new();
    protected StackPanel topBarRight;
    protected StackPanel topBarLeft;

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
            resizeManager?.ResizeSelectionBoxData(size);
        };
    }
    
    protected override void Initialize()
    {
        gum.Initialize(this);
        LoadUI();
        base.Initialize();
        
        resizeManager = new ResizeManager(new Vector2(gum.CanvasWidth, gum.CanvasHeight), graphics, Window);
    }
    
    protected virtual void LoadUI()
    {
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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        gum.Update(gameTime);
        base.Update(gameTime);
        
        if (ProdToolDOOM.Window.Helper.HasFocus(Window.Handle))
            CheckOnHover();

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

    private void CheckOnHover()
    {
        var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);
        
        // TODO Check any other resizable elements
        
        if (Fullscreen)
            return;
        resizeManager?.CheckResizePositions(mousePosition, mouseState);
        resizeManager?.ResizeWindow();
    }
}