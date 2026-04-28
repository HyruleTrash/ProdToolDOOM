using ProdToolDOOM.ProjectFeatures;

namespace ProdToolDOOM;

public class RemovePointCmd(Project project, Point tempPoint, Action? onExecuted) : ICommand
{
    private Point? point;
    
    public void Execute()
    {
        if (tempPoint == null)
            return;
        this.point ??= tempPoint;
        Debug.Log($"Removing point from level {this.point.LevelId}!");
        
        project.levels[this.point.LevelId].Remove(this.point);
        if (this.point.icon != null) this.point.Hide();
        onExecuted?.Invoke();
    }

    public void Undo()
    {
        if (project.levels.Count == 0 || project.CurrentLevel > project.levels.Count - 1)
            return;
        
        Debug.Log($"Adding point to level {this.point.LevelId} {this.point.Position}!");
        project.levels[this.point.LevelId].Add(this.point);
        if (this.point.icon != null) this.point.icon.Visible = true;
    }
}