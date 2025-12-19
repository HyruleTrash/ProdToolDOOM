namespace ProdToolDOOM;

public class Level
{
    public class Object
    {
        public Vector2 Position { get => position; set => position = value; }
        protected Vector2 position;
    }
    
    public List<Entity> Entities { get => entities; set => entities = value; }
    private List<Entity> entities = [];
    
    public List<Vector2> Points { get => points; }
    public List<Line> Lines { get => lines; }
    private List<Vector2> points = [];
    private List<Line> lines = [];

    public List<Object> levelObjects = [];

    public Level(Level? other = null)
    {
        if (other == null)
            return;
        entities = other.Entities;
        points = other.Points;
        lines = other.Lines;
    }

    public void SetLevelGeometry(List<Vector2> points, List<Line> lines)
    {
        this.points = points;
        this.lines = lines;
    }

    public void Add(Object? levelObject)
    {
        if (levelObject == null)
            return;
        levelObjects.Add(levelObject);
        switch (levelObject)
        {
            case Entity entity:
                entities.Add(entity);
                break;
            case Point point:
                points.Add(point.Position);
                break;
        }
    }

    public void Remove(Object? levelObject)
    {
        if (levelObject == null)
            return;
        levelObjects.Remove(levelObject);
        switch (levelObject)
        {
            case Entity entity:
                entities.Remove(entity);
                break;
            case Point point:
                points.Remove(point.Position);
                break;
        }
    }
}