using DLLevelBuilder.ProjectFeatures;

namespace DLLevelBuilder;

public class RemovePointCmd : RemoveLevelObjectCmd<Point>
{
    public RemovePointCmd(Project project, Point tempPoint, Action? onExecuted = null) : base(project, tempPoint, onExecuted) { }

    protected override void OnExecute()
    {
        Debug.Log($"Removing point from level {this.levelObj.LevelId}!");
        this.projectRef.levels[this.levelObj.LevelId].Remove(this.levelObj);
        if (this.levelObj.icon != null) this.levelObj.Hide();
    }

    protected override void OnUndo()
    {
        Debug.Log($"Adding point to level {this.levelObj.LevelId} {this.levelObj.Position}!");
        this.projectRef.levels[this.levelObj.LevelId].Add(this.levelObj);
        if (this.levelObj.icon != null) this.levelObj.Show();
    }
}