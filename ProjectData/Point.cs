using Accessibility;
using Gum.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.GueDeriving;
using ProdToolDOOM.ProjectFeatures;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM;

public class Point : Level.Object, IDisposable, IBaseUpdatable
{
    public List<Line> lines = [];
    public SpriteRuntime? icon;
    private SpriteRuntime? selectedIcon;
    private ContainerRuntime? iconContainer;
    public int LevelId { get; set; }
    public int LevelObjectId { get; set; }
    public Vector2 Position { get => this.position; set => this.position = value; }
    
    private Texture2D pointTextureRef;
    private bool beingMoved = false;
    
    private readonly WindowInstance windowRef;
    private readonly Project projectRef;

    public Point(Vector2 point, Texture2D pointTexture, int levelObjectId, int levelId, WindowInstance windowRef, Project projectRef)
    {
        this.windowRef = windowRef;
        this.projectRef = projectRef;
        this.position = point;
        this.pointTextureRef  = pointTexture;
        this.LevelObjectId = levelObjectId;
        this.LevelId = levelId;

        this.iconContainer = new ContainerRuntime
        {
            Width = this.pointTextureRef.Width,
            Height = this.pointTextureRef.Height,
            IgnoredByParentSize = true
        };
        this.icon = new SpriteRuntime
        {
            Texture = this.pointTextureRef,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = this.pointTextureRef.Width,
            TextureHeight = this.pointTextureRef.Height,
            IgnoredByParentSize = true,
            Visible = projectRef.CurrentLevel == levelId
        };
        this.iconContainer.AddChild(this.icon);
        this.selectedIcon = new SpriteRuntime
        {
            Texture = this.pointTextureRef,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = this.pointTextureRef.Width,
            TextureHeight = this.pointTextureRef.Height,
            IgnoredByParentSize = true,
            Visible = false
        };
        this.selectedIcon.Color = Color.Blue;
        this.iconContainer.AddChild(this.selectedIcon);
        
        if (this.icon.Visible)
            projectRef.canvasContainer.AddChild(this.iconContainer);

        this.iconContainer.RightClick += HandleRightClick;
        this.iconContainer.Dragging += HandleLeftClickHold;

        UpdateVisualPosition(windowRef.GetWindowSize());
        windowRef.onScreenSizeChange += UpdateVisualPosition;
        
        projectRef.onCurrentLevelChanged += OnLevelChanged;
    }

    private void OnLevelChanged(int newLevelId)
    {
        if (newLevelId != this.LevelId)
        {
            if (this.icon != null) this.icon.Visible = false;
            return;
        }

        if (this.iconContainer is { Parent: null }) this.projectRef.canvasContainer.AddChild(this.iconContainer);
        if (this.icon != null) this.icon.Visible = true;
    }

    private void HandleLeftClickHold(object? _, EventArgs __)
    {
        if (this.beingMoved) return;
        this.beingMoved = true;
        this.windowRef.Mouse.IsDragging = true;
        Program.instance.UpdateRegister.Add(this);
    }
    
    private void HandleRightClick(object? _, EventArgs __)
    {
        rightClickManager.instance.ShowOptions<Point>(new Vector2(this.windowRef.Mouse.currentMouseState.Position), this, 1);
    }

    public void UpdateVisualPosition(Vector2 screenSize)
    {
        if (this.icon == null) return;
        this.iconContainer.X = this.Position.x - (float)this.pointTextureRef.Width / 2 + screenSize.x / 2;
        this.iconContainer.Y = this.Position.y - (float)this.pointTextureRef.Height / 2 + screenSize.y / 2;
    }
    
    public void Update(float dt, WindowInstance _)
    {
        if (!this.beingMoved)
        {
            Program.instance.UpdateRegister.Remove(this);
            this.windowRef.Mouse.IsDragging = false;
            return;
        }
        MouseState mouse = this.windowRef.Mouse.currentMouseState;
        if (mouse.LeftButton == ButtonState.Released) this.beingMoved = false;
        this.Position = this.windowRef.Mouse.GetMousePosition();
        UpdateVisualPosition(this.windowRef.GetWindowSize());
    }

    public void Dispose()
    {
        if (this.icon != null) this.iconContainer.Parent = null;
    }

    public override void Hide()
    {
        if (this.selectedIcon != null) this.selectedIcon.Visible = false;
        if (this.icon != null) this.icon.Visible = false;
    }

    public override void ShowSelectionVisual()
    {
        if (this.selectedIcon != null) this.selectedIcon.Visible = true;
    }

    public override void HideSelectionVisual()
    {
        if (this.selectedIcon != null) this.selectedIcon.Visible = false;
    }

    public override string ToString()
    {
        return $"Point [position: {this.Position}, id: {this.LevelObjectId}, levelId: {this.LevelId}]";
    }
}