namespace ProdToolDOOM;

public class Point : Level.Object
{
    public List<Line> lines = [];
    
    public Point(Vector2 point)
    {
        position = point;
    }
}