using System.Collections.Generic;
using System.Linq;

namespace DLLevelBuilder;

public class Level
{
    public int IdCounter { get => this.levelObjectIdCounter; }
    public int LevelId { get => this.levelId; set => this.levelId = value; }
    private int levelId;
    
    public List<Entity> Entities { get => this.entities; set => this.entities = value; }
    private List<Entity> entities = [];
    
    public List<Point> Points { get => this.points; }
    public List<Line> Lines { get => this.lines; }
    
    private List<Point> points = [];
    private List<Line> lines = [];

    public List<LevelObject> levelObjects = [];
    public int levelObjectIdCounter = 0;
    
    private Vector2 objectOffset = Vector2.Zero;

    public Level(int id = -1) => this.levelId = id;

    public Level(Level? other)
    {
        if (other == null)
            return;
        this.entities = other.Entities;
        this.points = other.Points;
        this.lines = other.Lines;
        this.levelId = other.levelId;
    }

    public void Add(LevelObject? levelObject)
    {
        if (levelObject == null)
            return;
        this.levelObjects.Add(levelObject);
        switch (levelObject)
        {
            case Entity entity:
                this.entities.Add(entity);
                break;
            case Point point:
                this.points.Add(point);
                break;
            case Line line:
                this.lines.Add(line);
                break;
        }
    }

    public void Remove(LevelObject? levelObject)
    {
        if (levelObject == null)
            return;
        this.levelObjects.Remove(levelObject);
        switch (levelObject)
        {
            case Entity entity:
                this.entities.Remove(entity);
                break;
            case Point point:
                this.points.Remove(point);
                break;
            case Line line:
                this.lines.Remove(line);
                break;
        }
    }

    public Point? GetPointById(int id)
    {
        return this.points.FirstOrDefault(point => point.LevelObjectId == id);
    }

    public Vector2 GetOffset() => this.objectOffset;

    public void SetOffset(Vector2 newOffset)
    {
        this.objectOffset = newOffset;
        foreach (LevelObject levelObject in this.levelObjects) levelObject.UpdateVisualOffset(newOffset);
    }

    public void CheckInit()
    {
        foreach (Line line in this.Lines) line.Init();
        foreach (Point point in this.Points) point.Init();
        foreach (Entity entity in this.Entities) entity.Init();
    }
}