namespace ProdToolDOOM;

public class AddLinesCmd(Project project, Func<bool, Point[]> getPoints) : ICommand
{
    private Point[]? points;
    private List<AddLineCmd> actions = [];

    public void Execute()
    {
        this.points ??= getPoints.Invoke(false);

        if (this.points.Length < 2)
            return;

        if (this.points.Length == 2)
        {
            AddLineCmd cmd = new(project, this.points[0], this.points[1]);
            cmd.Execute();
            this.actions.Add(cmd);
            return;
        }

        // Sort points around centroid (clockwise)
        this.points = SortPointsCircular(this.points);

        for (int i = 0; i < this.points.Length; i++)
        {
            Point p1 = this.points[i];
            Point p2 = this.points[(i + 1) % this.points.Length];

            AddLineCmd cmd = new(project, p1, p2);
            cmd.Execute();
            this.actions.Add(cmd);
        }
    }

    public void Undo()
    {
        foreach (AddLineCmd action in this.actions)
            action.Undo();
    }

    private static Point[] SortPointsCircular(Point[] points)
    {
        Vector2 center = GetCenter(points);

        return points
            .OrderBy(p => Math.Atan2(p.position.y - center.y, p.position.x - center.x))
            .ToArray();
    }

    private static Vector2 GetCenter(Point[] points)
    {
        float x = 0;
        float y = 0;
        foreach (Point point in points)
        {
            x += point.position.x;
            y += point.position.y;
        }
        x /= points.Length;
        y /= points.Length;
        return new Vector2(x, y);
    }
}