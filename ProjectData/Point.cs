using Accessibility;
using Gum.Managers;
using Microsoft.Xna.Framework.Graphics;
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
    public Vector2 Position { get => position; set => position = value; }
    
    private Texture2D pointTextureRef;
    private bool beingMoved = false;
    
    private readonly WindowInstance windowRef;
    private readonly Project projectRef;

    public Point(Vector2 point, Texture2D pointTexture, int levelObjectId, int levelId, WindowInstance windowRef, Project projectRef)
    {
        this.windowRef = windowRef;
        this.projectRef = projectRef;
        position = point;
        pointTextureRef  = pointTexture;
        LevelObjectId = levelObjectId;
        LevelId = levelId;

        iconContainer = new ContainerRuntime
        {
            Width = pointTextureRef.Width,
            Height = pointTextureRef.Height,
            IgnoredByParentSize = true
        };
        icon = new SpriteRuntime
        {
            Texture = pointTextureRef,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = pointTextureRef.Width,
            TextureHeight = pointTextureRef.Height,
            IgnoredByParentSize = true,
            Visible = projectRef.CurrentLevel == levelId
        };
        iconContainer.AddChild(icon);
        selectedIcon = new SpriteRuntime
        {
            Texture = pointTextureRef,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = pointTextureRef.Width,
            TextureHeight = pointTextureRef.Height,
            IgnoredByParentSize = true,
            Visible = false
        };
        selectedIcon.Color = Color.Blue;
        iconContainer.AddChild(selectedIcon);
        
        if (icon.Visible)
            projectRef.canvasContainer.AddChild(iconContainer);

        iconContainer.RightClick += HandleRightClick;
        iconContainer.Dragging += HandleLeftClickHold;

        UpdateVisualPosition(windowRef.GetWindowSize());
        windowRef.onScreenSizeChange += UpdateVisualPosition;
        
        projectRef.onCurrentLevelChanged += OnLevelChanged;
    }

    private void OnLevelChanged(int newLevelId)
    {
        if (newLevelId != LevelId)
        {
            if (icon != null) icon.Visible = false;
            return;
        }

        if (iconContainer is { Parent: null })
            projectRef.canvasContainer.AddChild(iconContainer);
        if (icon != null) icon.Visible = true;
    }

    private void HandleLeftClickHold(object? _, EventArgs __)
    {
        if (beingMoved) return;
        beingMoved = true;
        windowRef.Mouse.IsDragging = true;
        Program.instance.UpdateRegister.Add(this);
    }
    
    private void HandleRightClick(object? _, EventArgs __)
    {
        rightClickManager.instance.ShowOptions<Point>(new Vector2(windowRef.Mouse.currentMouseState.Position), this, 1);
    }

    public void UpdateVisualPosition(Vector2 screenSize)
    {
        if (icon == null) return;
        iconContainer.X = Position.x - (float)pointTextureRef.Width / 2 + screenSize.x / 2;
        iconContainer.Y = Position.y - (float)pointTextureRef.Height / 2 + screenSize.y / 2;
    }
    
    public void Update(float dt, WindowInstance _)
    {
        if (!beingMoved)
        {
            Program.instance.UpdateRegister.Remove(this);
            windowRef.Mouse.IsDragging = false;
            return;
        }
        var mouse = windowRef.Mouse.currentMouseState;
        if (mouse.LeftButton == ButtonState.Released)
            beingMoved = false;
        Position = windowRef.Mouse.GetMousePosition();
        UpdateVisualPosition(windowRef.GetWindowSize());
    }

    public void Dispose()
    {
        if (icon != null) iconContainer.Parent = null;
    }

    public override void Hide()
    {
        if (selectedIcon != null) selectedIcon.Visible = false;
        if (icon != null) icon.Visible = false;
    }

    public override void ShowSelectionVisual()
    {
        if (selectedIcon != null) selectedIcon.Visible = true;
    }

    public override void HideSelectionVisual()
    {
        if (selectedIcon != null) selectedIcon.Visible = false;
    }

    public override string ToString()
    {
        return $"Point [position: {Position}, id: {LevelObjectId}, levelId: {LevelId}]";
    }
}