namespace ProdToolDOOM;

public class RemoveLineCmd : RemoveLevelObjectCmd<Line>
{
    public RemoveLineCmd(Project project, Line tempLine, Action? onExecuted = null) : base(project, tempLine, onExecuted) { }

    protected override void OnExecute()
    {
        Debug.Log($"Removing line from level {this.levelObj.LevelId}!");
        this.projectRef.levels[this.levelObj.LevelId].Remove(this.levelObj);
        if (this.levelObj.icon != null) this.levelObj.Hide();
    }

    protected override void OnUndo()
    {
        Debug.Log($"Adding line to level {this.levelObj.LevelId}!");
        this.projectRef.levels[this.levelObj.LevelId].Add(this.levelObj);
        if (this.levelObj.icon != null) this.levelObj.Show();
    }
}