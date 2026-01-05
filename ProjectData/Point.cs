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
    public int LevelId { get; private set; }
    private Texture2D pointTextureRef;
    private bool beingMoved = false;
    
    private readonly WindowInstance windowRef;
    private readonly Project projectRef;
    
    public Point(Vector2 point, Texture2D pointTexture, int levelId, WindowInstance windowRef, Project projectRef)
    {
        this.windowRef = windowRef;
        this.projectRef = projectRef;
        position = point;
        pointTextureRef  = pointTexture;

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
            Visible = projectRef.currentLevel == levelId
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
        Program.instance.onScreenSizeChange += UpdateVisualPosition;
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

    private void UpdateVisualPosition(Vector2 screenSize)
    {
        if (icon == null) return;
        iconContainer.X = Position.x - (float)pointTextureRef.Width / 2 + screenSize.x / 2;
        iconContainer.Y = Position.y - (float)pointTextureRef.Height / 2 + screenSize.y / 2;
    }
    
    public void UpdatePosition(Vector2 newPosition) => projectRef.levels[LevelId].UpdatePointPosition(this, newPosition);
    
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
        UpdatePosition(windowRef.Mouse.GetMousePosition());
        UpdateVisualPosition(windowRef.GetWindowSize());
    }

    public void Dispose()
    {
        if (icon != null) iconContainer.Parent = null;
    }

    public void Hide()
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
}