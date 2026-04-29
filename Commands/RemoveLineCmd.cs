namespace ProdToolDOOM;

public class RemoveLineCmd(Project project, Line tempLine, Action? onExecuted = null) : ICommand
{
    private Line? line;
    
    public void Execute()
    {
        if (tempLine == null)
            return;
        this.line ??= tempLine;
        Debug.Log($"Removing line from level {this.line.LevelId}!");
        
        project.levels[this.line.LevelId].Remove(this.line);
        if (this.line.icon != null) this.line.Hide();
        onExecuted?.Invoke();
    }

    public void Undo()
    {
        if (project.levels.Count == 0 || project.CurrentLevel > project.levels.Count - 1)
            return;
        
        Debug.Log($"Adding line to level {this.line.LevelId}!");
        project.levels[this.line.LevelId].Add(this.line);
        if (this.line.icon != null) this.line.Show();
    }
}