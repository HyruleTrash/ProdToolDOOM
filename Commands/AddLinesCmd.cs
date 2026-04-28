namespace ProdToolDOOM;

public class AddLinesCmd(Project project, Func<bool, Point[]> getPoints) : ICommand
{
    private Point[]? points;
    private List<AddLineCmd> actions = [];
    
    public void Execute()
    {
        points ??= getPoints.Invoke(false);

        if (points.Length == 0)
            return;
        if (points.Length == 2)
        {
            var cmd = new AddLineCmd(project, points[0], points[1]);
            cmd.Execute();
            actions.Add(cmd);
            return;
        }
        
        // TODO sort points based on circle around middle position

        for (var index = 0; index < points.Length; index++)
        {
            var point1 = points[index];
            var point2 = points[index + 1 == points.Length - 1 ? 0 : index + 1];
            var cmd = new AddLineCmd(project, point1, point2);
            cmd.Execute();
            actions.Add(cmd);
        }
    }

    public void Undo()
    {
        foreach (var action in actions) action.Undo();
    }
}