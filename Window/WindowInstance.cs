using System;
using System.Collections.Generic;
using System.Linq;
using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DLLevelBuilder.ProjectFeatures;
using Gum.DataTypes;
using Gum.Forms.DefaultVisuals;
using static Microsoft.Xna.Framework.Color;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace DLLevelBuilder;

public class WindowInstance : Game
{
    protected GraphicsDeviceManager graphics;
    protected GumService gum => GumService.Default;
    
    public Action<Vector2>? onScreenSizeChange;
    public bool Fullscreen { get; private set; }

    private bool shouldCallOnScreenSizeChanged;
    private Window.ResizeComponent? resizeComponent;
    private Window.DragComponent? dragComponent;

    protected Menu TopBarRight { get; private set; }
    protected Menu topBarLeft;
    
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

        this.topBarLeft = new Menu
        {
            X = UIParams.borderPadding,
            Y = UIParams.borderPadding,
            Height = UIParams.minButtonHeight,
            Visual = { WidthUnits = DimensionUnitType.RelativeToChildren, }
        };
        if (this.topBarLeft.Visual is MenuVisual topBarLeftMenuVisual)
            topBarLeftMenuVisual.Background.Color = Transparent;
        this.topBarLeft.Anchor(Anchor.TopLeft);

        this.TopBarRight = new Menu
        {
            Y = UIParams.borderPadding,
            Height = UIParams.minButtonHeight,
            Visual = { WidthUnits = DimensionUnitType.RelativeToChildren, }
        };
        if (this.TopBarRight.Visual is MenuVisual menuVisual)
        {
            menuVisual.Background.Color = Transparent;
            menuVisual.InnerPanelInstance.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Right;
            menuVisual.InnerPanelInstance.XUnits = Gum.Converters.GeneralUnitType.PixelsFromLarge;
            menuVisual.InnerPanelInstance.StackSpacing = 0;
        }
        this.TopBarRight.Anchor(Anchor.TopLeft);

        this.onScreenSizeChange += _ => UpdateTopBars();

        this.topBarLeft.ItemsCollectionChanged += (_, _) => UpdateUIMenu(this.topBarLeft);
        this.TopBarRight.ItemsCollectionChanged += (_, _) => UpdateTopBarRightUI();
    }

    public void UpdateTopBars()
    {
        UpdateUIMenu(this.topBarLeft);
        UpdateTopBarRightUI();
    }

    private void UpdateTopBarRightUI()
    {
        float rightWidth = GetFullMenuWidth(this.TopBarRight);
        this.TopBarRight.Width = rightWidth;
        MenuVisual menuVisual = (MenuVisual)this.TopBarRight.Visual;
        float padding = menuVisual.InnerPanelInstance.StackSpacing;
        menuVisual.XUnits = Gum.Converters.GeneralUnitType.PixelsFromLarge;
        menuVisual.X = padding - rightWidth;
        UpdateUIMenu(this.TopBarRight);
    }

    private void UpdateUIMenu(Menu menu)
    {
        foreach (MenuItem item in menu.MenuItems) item.Visual.UpdateLayout();
        menu.Visual.UpdateLayout();
        menu.Width = GetFullMenuWidth(menu);
        menu.Visual.UpdateLayout();
    }
    
    private float GetFullMenuWidth(Menu menu)
    {
        float fullWidth = 0;
        MenuVisual menuVisual = (MenuVisual)menu.Visual;
        float padding = menuVisual.InnerPanelInstance.StackSpacing;
        foreach (MenuItem item in menu.MenuItems)
        {
            if (!item.IsVisible) continue;
            MenuItemVisual itemVisual = (MenuItemVisual)item.Visual;

            float baseContainerWidth = itemVisual.GetAbsoluteWidth();
            float arrowOffset = 0;
            
            if (itemVisual.SubmenuIndicatorInstance.Visible)
                arrowOffset = itemVisual.SubmenuIndicatorInstance.X;

            float realCalculatedItemWidth = baseContainerWidth + arrowOffset + padding;
            fullWidth += realCalculatedItemWidth;
        }
        return fullWidth;
    }
    
    protected virtual void LoadUI()
    {
        MenuItem exitButton = new();
        UIParams.SetDefaultMenuItem(exitButton);
        UIParams.AddIconToMenuItem(exitButton, this.closeIcon);
        exitButton.Clicked += (_, _) => Exit();
        
        MenuItem minimizeButton = new();
        UIParams.SetDefaultMenuItem(minimizeButton);
        UIParams.AddIconToMenuItem(minimizeButton, this.minimizeIcon);
        minimizeButton.Clicked += (_, _) =>
        {
            IntPtr handle = this.Window.Handle;
            if (handle == IntPtr.Zero) return;
            DLLevelBuilder.Window.Helper.Minimize(handle);
        };
        
        MenuItem maximizeButton = new();
        UIParams.SetDefaultMenuItem(maximizeButton);
        UIParams.AddIconToMenuItem(maximizeButton, this.maximizeIcon);
        maximizeButton.Clicked += (_, _) =>
        {
            IntPtr handle = this.Window.Handle;
            if (handle == IntPtr.Zero) return;

            if (this.Fullscreen)
            {
                DLLevelBuilder.Window.Helper.UnMaximize(handle);
                this.Fullscreen = false;
            }
            else
            {
                DLLevelBuilder.Window.Helper.Maximize(handle);
                this.Fullscreen = true;
            }

            this.shouldCallOnScreenSizeChanged = true;
        };
        this.TopBarRight.Items.Add(maximizeButton);
        this.TopBarRight.Items.Add(minimizeButton);
        this.TopBarRight.Items.Add(exitButton);
    }

    private void FinalizeUI()
    {
        this.dragComponent?.FinalizeUI();
        this.TopBarRight.AddToRoot();
        this.topBarLeft.AddToRoot();
        UpdateTopBars();

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
        
        if (this.Fullscreen || WasMouseClickConsumedByGum())
            return;
        
        bool? dragging = this.dragComponent?.CheckHover(mouseState, dt);
        if (dragging is not (null or false)) return;
        this.resizeComponent?.CheckHover(mouseState, dt);
        this.resizeComponent?.ResizeWindow();
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

    public bool IsFocused() => DLLevelBuilder.Window.Helper.HasFocus(this.Window.Handle);

    public Vector2 GetWindowSize()
    {
        return new Vector2(GetWindowWidth(), GetWindowHeight());
    }
    
    public float GetWindowWidth() => this.Window.ClientBounds.Width;
    public float GetWindowHeight() => this.Window.ClientBounds.Height;
}