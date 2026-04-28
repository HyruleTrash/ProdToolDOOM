using MonoGameGum.GueDeriving;

namespace ProdToolDOOM;

public class Level
{
    public abstract class Object
    {
        public Vector2 position;
        public bool visible = true;
        public Action onShowEvent;
        public Action onHideEvent;

        public void Show()
        {
            OnShow();
            this.visible = true;
            this.onShowEvent?.Invoke();
        }

        public void Hide()
        {
            OnHide();
            this.visible = false;
            this.onHideEvent?.Invoke();
        }
        protected abstract void OnShow();
        protected abstract void OnHide();
        public abstract void ShowSelectionVisual();
        public abstract void HideSelectionVisual();
    }
    
    public int IdCounter { get => this.levelObjectIdCounter; }
    public List<Entity> Entities { get => this.entities; set => this.entities = value; }
    private List<Entity> entities = [];
    
    public List<Point> Points { get => this.points; }
    public List<Line> Lines { get => this.lines; }
    
    private List<Point> points = [];
    private List<Line> lines = [];

    public List<Object> levelObjects = [];
    public int levelObjectIdCounter = 0;

    public Level(Level? other = null)
    {
        if (other == null)
            return;
        this.entities = other.Entities;
        this.points = other.Points;
        this.lines = other.Lines;
    }

    public void Add(Object? levelObject)
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

    public void Remove(Object? levelObject)
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
}