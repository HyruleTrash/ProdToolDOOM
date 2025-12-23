using ProdToolDOOM.ProjectFeatures;

namespace ProdToolDOOM;

public class RemovePointCmd(Project project) : ICommand
{
    private Point? point;
    
    public void Execute()
    {
        if (rightClickManager.instance.currentSelection is not Point tempPoint)
            return;
        point = tempPoint;
        Debug.Log($"Removing point from level {point.LevelId}!");
        
        project.levels[point.LevelId].Remove(point);
        if (point.icon != null) point.icon.Visible = false;
        rightClickManager.instance.Reset();
    }

    public void Undo()
    {
        if (project.levels.Count == 0 || project.currentLevel > project.levels.Count - 1)
            return;
        
        Debug.Log($"Adding point to level {point.LevelId} {point.Position}!");
        project.levels[point.LevelId].Add(point);
        if (point.icon != null) point.icon.Visible = true;
    }
}