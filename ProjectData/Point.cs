using Accessibility;
using Gum.Managers;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;
using ProdToolDOOM.ProjectFeatures;
using Color = Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM;

public class Point : Level.Object, IDisposable
{
    public List<Line> lines = [];
    public SpriteRuntime? icon;
    public SpriteRuntime? selectedIcon;
    public ContainerRuntime? iconContainer;
    public int LevelId { get; private set; }
    private Texture2D pointTextureRef;
    
    public Point(Vector2 point, Texture2D pointTexture, int levelId)
    {
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
            Visible = Project.instance.currentLevel == levelId
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
            Project.instance.canvasContainer.AddChild(iconContainer);

        iconContainer.RightClick += (_, __) => 
            rightClickManager.instance.ShowOptions<Point>(point + new Vector2(Program.GetWindowWidth(), Program.GetWindowHeight()) / 2, this, 1);

        UpdatePosition(new Vector2(Program.GetWindowWidth(), Program.GetWindowHeight()));
        Program.instance.onScreenSizeChange += UpdatePosition;
    }

    private void UpdatePosition(Vector2 screenSize)
    {
        if (icon == null) return;
        iconContainer.X = Position.x - (float)pointTextureRef.Width / 2 + screenSize.x / 2;
        iconContainer.Y = Position.y - (float)pointTextureRef.Height / 2 + screenSize.y / 2;
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