using ProdToolDOOM.ProjectFeatures.Tools;

namespace ProdToolDOOM;

public class RemovePointsCmd(Project project, Func<bool, Point[]> getPoints) : ICommand
{
    private Point[]? points;
    private List<RemovePointCmd> actions = [];
    
    public void Execute()
    {
        this.points ??= getPoints.Invoke(true);
        foreach (Point point in this.points)
        {
            RemovePointCmd cmd = new RemovePointCmd(project, point, null);
            cmd.Execute();
            this.actions.Add(cmd);
        }
    }

    public void Undo()
    {
        foreach (RemovePointCmd action in this.actions) action.Undo();
    }
}