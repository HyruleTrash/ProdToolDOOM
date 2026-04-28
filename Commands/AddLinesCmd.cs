namespace ProdToolDOOM;

public class AddLinesCmd(Project project, Func<bool, Point[]> getPoints) : ICommand
{
    private Point[]? points;
    private List<AddLineCmd> actions = [];
    
    public void Execute()
    {
        this.points ??= getPoints.Invoke(false);

        if (this.points.Length == 0)
            return;
        if (this.points.Length == 2)
        {
            AddLineCmd cmd = new AddLineCmd(project, this.points[0], this.points[1]);
            cmd.Execute();
            this.actions.Add(cmd);
            return;
        }
        
        // TODO sort points based on circle around middle position

        for (int index = 0; index < this.points.Length; index++)
        {
            Point point1 = this.points[index];
            Point point2 = this.points[index + 1 == this.points.Length - 1 ? 0 : index + 1];
            AddLineCmd cmd = new AddLineCmd(project, point1, point2);
            cmd.Execute();
            this.actions.Add(cmd);
        }
    }

    public void Undo()
    {
        foreach (AddLineCmd action in this.actions) action.Undo();
    }
}