using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;

namespace ProdToolDOOM;

public class AddPointCmd(Project project, Vector2 initialPosition, Texture2D pointTexture, WindowInstance windowRef) : ICommand, IDisposable
{
    private Point? point;
    
    public void Execute()
    {
        if (project.levels.Count == 0 || project.currentLevel > project.levels.Count - 1)
            return;
        int levelId = project.currentLevel;
        point ??= new Point(initialPosition, pointTexture, levelId, windowRef, project);
        
        Debug.Log($"Adding point to level {levelId} {point.Position}!");
        project.levels[levelId].Add(point);
    }

    public void Undo()
    {
        if (point == null)
            return;
        Debug.Log($"Removing point from level {point.LevelId}!");
        
        project.levels[point.LevelId].Remove(point);
        if (point.icon != null) point.icon.Visible = false;
    }

    public void Dispose()
    {
        if (point?.icon?.Visible == false)
            point?.Dispose();
    }
}