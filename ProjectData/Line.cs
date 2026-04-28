using MonoGameGum.GueDeriving;
using Color = Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM;

public class Line : Level.Object
{
    public int Id { get => point1Id; set => point1Id = value; }
    public int IdOther { get => point2Id; set => point2Id = value; }
    public int LevelId { get => levelId; set => levelId = value; }
    public int levelId;
    private int point1Id = -1;
    private int point2Id = -1;
    
    private Vector2 midPoint = Vector2.Zero;
    
    public PolygonRuntime? icon;
    private PolygonRuntime? selectedIcon;
    private ContainerRuntime? iconContainer;
    
    private Project projectRef;

    public Line(Project projectRef, int point1Id, int point2Id, int levelId) : this(projectRef)
    {
        this.levelId = levelId;
        this.point1Id = point1Id;
        this.point2Id = point2Id;
        this.projectRef = projectRef;
        Init();
    }
    public Line(Project projectRef, Line line) : this(projectRef)
    {
        levelId = line.levelId;
        point1Id = line.Id;
        point2Id = line.IdOther;
        Init();
    }
    public Line(Project projectRef) => this.projectRef = projectRef;

    public void Init()
    {
        Point? point1 = projectRef.levels[levelId].GetPointById(point1Id);
        Point? point2 = projectRef.levels[levelId].GetPointById(point2Id);
        if (point1 == null || point2 == null)
            return;
        point1.lines.Add(this);
        point2.lines.Add(this);
        
        var screenSize = Program.instance.GetWindowSize();
        var point1Pos = point1.position;
        var point2Pos = point2.position;

        var distance = Vector2.GetDistance(point1Pos, point2Pos);
        var direction = Vector2.GetDirection(point1Pos, point2Pos);
        midPoint = point1Pos + direction * (distance / 2f);
        
        iconContainer = new ContainerRuntime
        {
            X = midPoint.x,
            Y = midPoint.y,
            Width = 25,
            Height = 25,
            IgnoredByParentSize = true
        };

        #region LineRenderer
        ICollection<System.Numerics.Vector2> polygonPoints = new List<System.Numerics.Vector2>
        {
            new (point1Pos.x - midPoint.x, point1Pos.y - midPoint.y),
            new (point2Pos.x - midPoint.x, point2Pos.y - midPoint.y)
        };
        icon = new PolygonRuntime
        {
            IgnoredByParentSize = true,
            Visible = projectRef.CurrentLevel == levelId,
            LineWidth = UIParams.defaultOutLineWidth,
            IsDotted = true,
        };
        icon.SetPoints(polygonPoints);
        iconContainer.AddChild(icon);
        
        selectedIcon = new PolygonRuntime
        {
            IgnoredByParentSize = true,
            Visible = false,
            LineWidth = UIParams.defaultOutLineWidth,
            IsDotted = true,
            Color = Color.Blue
        };
        selectedIcon.SetPoints(polygonPoints);
        iconContainer.AddChild(selectedIcon);
        #endregion
        
        if (icon.Visible)
            projectRef.canvasContainer.AddChild(iconContainer);
        
        UpdateVisualPosition(screenSize);
        Program.instance.onScreenSizeChange += UpdateVisualPosition;
    }
    
    public void UpdateVisualPosition(Vector2 screenSize)
    {
        if (iconContainer == null) return;
        iconContainer.X = midPoint.x + screenSize.x / 2;
        iconContainer.Y = midPoint.y + screenSize.y / 2;
    }
    
    public override void Hide()
    {
        if (selectedIcon != null) selectedIcon.Visible = false;
        if (icon != null) icon.Visible = false;
    }

    public override void ShowSelectionVisual()
    {
        if (selectedIcon != null) selectedIcon.Visible = true;
    }

    public override void HideSelectionVisual()
    {
        if (selectedIcon != null) selectedIcon.Visible = false;
    }
}