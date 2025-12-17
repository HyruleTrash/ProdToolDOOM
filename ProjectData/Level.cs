namespace ProdToolDOOM;

public class Line
{
    public int Id { get => id; set => id = value; }
    public int IdOther { get => idOther; set => idOther = value; }
    private int id;
    private int idOther;

    public Line(int id, int idOther)
    {
        this.id = id;
        this.idOther = idOther;
    }
    public Line(Line line)
    {
        id = line.Id;
        idOther = line.IdOther;
    }
    public Line(){}
}

public class Level
{
    public List<Entity> Entities { get => entities; set => entities = value; }
    private List<Entity> entities = [];
    
    public List<Vector2> Points { get => points; }
    public List<Line> Lines { get => lines; }
    private List<Vector2> points = [];
    private List<Line> lines = [];

    public Level()
    { }
    
    public Level(Level other)
    {
        entities = other.Entities;
        points = other.Points;
        lines = other.Lines;
    }

    public void SetLevelGeometry(List<Vector2> points, List<Line> lines)
    {
        this.points = points;
        this.lines = lines;
    }
}