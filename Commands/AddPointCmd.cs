using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;

namespace ProdToolDOOM;

public class AddPointCmd(Project project, Vector2 initialPosition, Texture2D pointTexture) : ICommand, IDisposable
{
    private Point? point;
    private int? levelId;
    private SpriteRuntime? icon;
    
    public void Execute()
    {
        if (project.levels.Count == 0 || project.currentLevel > project.levels.Count - 1)
            return;
        levelId ??= project.currentLevel;
        point ??= new Point(initialPosition);
        
        Debug.Log($"Adding point to level {levelId} {point.Position}!");
        project.levels[levelId.Value].Add(point);
        
        if (icon == null)
        {
            icon = new SpriteRuntime
            {
                Texture = pointTexture,
                TextureAddress = TextureAddress.Custom,
                TextureWidth = pointTexture.Width,
                TextureHeight = pointTexture.Height,
                IgnoredByParentSize = true
            };
            // icon.Anchor(Anchor.Center);
            project.canvasContainer.AddChild(icon);
        }

        icon.Visible = true;
        icon.X = point.Position.x - (float)pointTexture.Width / 2;
        icon.Y = point.Position.y - (float)pointTexture.Height / 2;
    }

    public void Undo()
    {
        if (point == null || levelId == null)
            return;
        Debug.Log($"Removing point from level {levelId}!");
        
        project.levels[levelId.Value].Remove(point);
        if (icon != null) icon.Visible = false;
    }

    public void Dispose()
    {
        if (icon != null) icon.Parent = null;
    }
}