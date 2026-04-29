namespace ProdToolDOOM;

public class RemoveLinesCmd(Project project, Func<bool, Line[]> getLines) : ICommand
{
    private Line[]? lines;
    private List<RemoveLineCmd> actions = [];
    
    public void Execute()
    {
        this.lines ??= getLines.Invoke(true);
        foreach (Line line in this.lines)
        {
            RemoveLineCmd cmd = new(project, line, null);
            cmd.Execute();
            this.actions.Add(cmd);
        }
    }

    public void Undo()
    {
        foreach (RemoveLineCmd action in this.actions) action.Undo();
    }
}