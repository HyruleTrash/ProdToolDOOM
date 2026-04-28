using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;

namespace ProdToolDOOM;

public class AddPointCmd(Project projectRef, Vector2 initialPosition, Texture2D pointTexture, WindowInstance windowRef) : ICommand, IDisposable
{
    private Point? point;
    
    public void Execute()
    {
        if (projectRef.levels.Count == 0 || projectRef.CurrentLevel > projectRef.levels.Count - 1)
            return;
        var levelId = projectRef.CurrentLevel;
        point ??= new Point(initialPosition, pointTexture, projectRef.levels[levelId].levelObjectIdCounter++, levelId, windowRef, projectRef);
        
        Debug.Log($"Adding point to level {levelId} {point.position}!");
        projectRef.levels[levelId].Add(point);
    }

    public void Undo()
    {
        if (point == null)
            return;
        Debug.Log($"Removing point from level {point.LevelId}!");
        
        projectRef.levels[point.LevelId].Remove(point);
        if (point.icon != null) point.icon.Visible = false;
    }

    public void Dispose()
    {
        if (point?.icon?.Visible == false)
            point?.Dispose();
    }
}