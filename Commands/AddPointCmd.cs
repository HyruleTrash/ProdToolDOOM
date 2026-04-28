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
        int levelId = projectRef.CurrentLevel;
        this.point ??= new Point(initialPosition, pointTexture, projectRef.levels[levelId].levelObjectIdCounter++, levelId, windowRef, projectRef);
        this.point.Init();
        
        Debug.Log($"Adding point to level {levelId} {this.point.position}!");
        projectRef.levels[levelId].Add(this.point);
    }

    public void Undo()
    {
        if (this.point == null)
            return;
        Debug.Log($"Removing point from level {this.point.LevelId}!");
        
        projectRef.levels[this.point.LevelId].Remove(this.point);
        if (this.point.icon != null) this.point.Hide();
    }

    public void Dispose()
    {
        if (this.point?.icon?.Visible == false) this.point?.Dispose();
    }
}