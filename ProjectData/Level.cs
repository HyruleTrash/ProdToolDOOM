using MonoGameGum.GueDeriving;

namespace ProdToolDOOM;

public class Level
{
    public abstract class Object
    {
        public Vector2 position;

        public abstract void Hide();
        public abstract void ShowSelectionVisual();
        public abstract void HideSelectionVisual();
    }
    
    public int IdCounter { get => levelObjectIdCounter; }
    public List<Entity> Entities { get => entities; set => entities = value; }
    private List<Entity> entities = [];
    
    public List<Point> Points { get => points; }
    public List<Line> Lines { get => lines; }
    
    private List<Point> points = [];
    private List<Line> lines = [];

    public List<Object> levelObjects = [];
    public int levelObjectIdCounter = 0;

    public Level(Level? other = null)
    {
        if (other == null)
            return;
        entities = other.Entities;
        points = other.Points;
        lines = other.Lines;
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
                points.Add(point);
                break;
            case Line line:
                lines.Add(line);
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
                points.Remove(point);
                break;
            case Line line:
                lines.Remove(line);
                break;
        }
    }

    public Point? GetPointById(int id)
    {
        return points.FirstOrDefault(point => point.LevelObjectId == id);
    }
}