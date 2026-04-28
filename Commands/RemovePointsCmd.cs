using ProdToolDOOM.ProjectFeatures.Tools;

namespace ProdToolDOOM;

public class RemovePointsCmd(Project project, Func<bool, Point[]> getPoints) : ICommand
{
    private Point[]? points;
    private List<RemovePointCmd> actions = [];
    
    public void Execute()
    {
        points ??= getPoints.Invoke(true);
        foreach (var point in points)
        {
            var cmd = new RemovePointCmd(project, point, null);
            cmd.Execute();
            actions.Add(cmd);
        }
    }

    public void Undo()
    {
        foreach (var action in actions) action.Undo();
    }
}