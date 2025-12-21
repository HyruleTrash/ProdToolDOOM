using Gum.Managers;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;

namespace ProdToolDOOM;

public class Point : Level.Object, IDisposable
{
    public List<Line> lines = [];
    public SpriteRuntime? icon;
    public int LevelId { get; private set; }
    
    public Point(Vector2 point, Texture2D pointTexture, int levelId)
    {
        position = point;
        
        icon = new SpriteRuntime
        {
            Texture = pointTexture,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = pointTexture.Width,
            TextureHeight = pointTexture.Height,
            IgnoredByParentSize = true
        };
        icon.Visible = Project.instance.currentLevel == levelId;
        if (icon.Visible)
            Project.instance.canvasContainer.AddChild(icon);

        icon.X = Position.x - (float)pointTexture.Width / 2;
        icon.Y = Position.y - (float)pointTexture.Height / 2;
    }

    public void Dispose()
    {
        if (icon != null) icon.Parent = null;
    }
}