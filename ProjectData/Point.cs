using Accessibility;
using Gum.Managers;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;
using ProdToolDOOM.ProjectFeatures;

namespace ProdToolDOOM;

public class Point : Level.Object, IDisposable
{
    public List<Line> lines = [];
    public SpriteRuntime? icon;
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
        
        if (icon.Visible)
            Project.instance.canvasContainer.AddChild(iconContainer);

        iconContainer.RightClick += (_, __) =>
        {
            rightClickManager.instance.currentSelection = this;
            rightClickManager.instance.ShowOptions<Point>(point + new Vector2(Program.GetWindowWidth(), Program.GetWindowHeight()) / 2);
        };

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
}