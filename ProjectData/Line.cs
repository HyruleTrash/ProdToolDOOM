using MonoGameGum.GueDeriving;
using Color = Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM;

public class Line : Level.Object, IDisposable
{
    public int Id { get => this.point1Id; set => this.point1Id = value; }
    public int IdOther { get => this.point2Id; set => this.point2Id = value; }
    public int LevelId { get => this.levelId; set => this.levelId = value; }
    public int levelId;
    private int point1Id = -1;
    private int point2Id = -1;
    
    private Vector2 midPoint = Vector2.Zero;
    
    public PolygonRuntime? icon;
    private PolygonRuntime? selectedIcon;
    private ContainerRuntime? iconContainer;
    
    private Project projectRef;
    private RemoveLineCmd removeCommand;
    public const float wallHeight = 5f;

    public Line(Project projectRef, int point1Id, int point2Id, int levelId) : this(projectRef)
    {
        this.levelId = levelId;
        this.point1Id = point1Id;
        this.point2Id = point2Id;
        this.projectRef = projectRef;
    }
    public Line(Project projectRef, Line line) : this(projectRef)
    {
        this.levelId = line.levelId;
        this.point1Id = line.Id;
        this.point2Id = line.IdOther;
    }
    public Line(Project projectRef) => this.projectRef = projectRef;

    public void Init()
    {
        Point? point1 = this.projectRef.levels[this.levelId].GetPointById(this.point1Id);
        Point? point2 = this.projectRef.levels[this.levelId].GetPointById(this.point2Id);
        if (point1 == null || point2 == null)
            return;
        point1.lines.Add(this);
        point2.lines.Add(this);
        
        Vector2 screenSize = Program.instance.GetWindowSize();
        Vector2 point1Pos = point1.position;
        Vector2 point2Pos = point2.position;

        float distance = Vector2.GetDistance(point1Pos, point2Pos);
        Vector2 direction = Vector2.GetDirection(point1Pos, point2Pos);
        this.midPoint = point1Pos + direction * (distance / 2f);
        this.position = this.midPoint;

        this.iconContainer = new ContainerRuntime
        {
            X = this.midPoint.x,
            Y = this.midPoint.y,
            Width = 25,
            Height = 25,
            IgnoredByParentSize = true
        };

        #region LineRenderer
        ICollection<System.Numerics.Vector2> polygonPoints = new List<System.Numerics.Vector2>
        {
            new (point1Pos.x - this.midPoint.x, point1Pos.y - this.midPoint.y),
            new (point2Pos.x - this.midPoint.x, point2Pos.y - this.midPoint.y)
        };
        this.icon = new PolygonRuntime
        {
            IgnoredByParentSize = true,
            Visible = this.projectRef.CurrentLevel == this.levelId,
            LineWidth = UIParams.defaultOutLineWidth,
            IsDotted = true,
        };
        this.icon.SetPoints(polygonPoints);
        this.iconContainer.AddChild(this.icon);

        this.selectedIcon = new PolygonRuntime
        {
            IgnoredByParentSize = true,
            Visible = false,
            LineWidth = UIParams.defaultOutLineWidth,
            IsDotted = true,
            Color = Color.Blue
        };
        this.selectedIcon.SetPoints(polygonPoints);
        this.iconContainer.AddChild(this.selectedIcon);
        #endregion
        
        if (this.icon.Visible) this.projectRef.canvasContainer.AddChild(this.iconContainer);
        
        UpdateVisualPosition(screenSize);
        Program.instance.onScreenSizeChange += UpdateVisualPosition;
        
        point1.onDispose += EvaluateVisibility;
        point1.onShowEvent += EvaluateVisibility;
        point1.onHideEvent += EvaluateVisibility;
        point1.onVisualMoved += EvaluatePolygonVisual;
        
        point2.onDispose += EvaluateVisibility;
        point2.onShowEvent += EvaluateVisibility;
        point2.onHideEvent += EvaluateVisibility;
        point2.onVisualMoved += EvaluatePolygonVisual;
    }
    
    public void UpdateVisualPosition(Vector2 screenSize)
    {
        if (this.iconContainer == null) return;
        this.iconContainer.X = this.midPoint.x + screenSize.x / 2;
        this.iconContainer.Y = this.midPoint.y + screenSize.y / 2;
    }
    
    public void Dispose()
    {
        if (this.icon == null) return;
        if (this.iconContainer != null)
            this.iconContainer.Parent = null;
        
        Point? point1 = this.projectRef.levels[this.levelId].GetPointById(this.point1Id);
        Point? point2 = this.projectRef.levels[this.levelId].GetPointById(this.point2Id);

        try
        {
            if (point1 != null)
            {
                point1.onDispose -= EvaluateVisibility;
                point1.onShowEvent -= EvaluateVisibility;
                point1.onHideEvent -= EvaluateVisibility;
                point1.onVisualMoved -= EvaluatePolygonVisual;
                return;
            }

            if (point2 == null) return;
            point2.onDispose -= EvaluateVisibility;
            point2.onShowEvent -= EvaluateVisibility;
            point2.onHideEvent -= EvaluateVisibility;
            point2.onVisualMoved -= EvaluatePolygonVisual;
        }
        catch (Exception _)
        {
            // ignored
        }
    }

    protected override void OnShow()
    {
        if (this.icon != null) this.icon.Visible = true;
    }

    protected override void OnHide()
    {
        if (this.selectedIcon != null) this.selectedIcon.Visible = false;
        if (this.icon != null) this.icon.Visible = false;
    }

    public override void ShowSelectionVisual()
    {
        if (this.selectedIcon != null) this.selectedIcon.Visible = true;
    }

    public override void HideSelectionVisual()
    {
        if (this.selectedIcon != null) this.selectedIcon.Visible = false;
    }

    public void EvaluateVisibility()
    {
        Point? point1 = this.projectRef.levels[this.levelId].GetPointById(this.point1Id);
        Point? point2 = this.projectRef.levels[this.levelId].GetPointById(this.point2Id);
        if (point1 == null || point2 == null)
        {
            CreateRemoveCommand();
            this.removeCommand.Execute();
            return;
        }
        Debug.Log($"{point1.visible}, {point2.visible}");
        if (point1.visible && point2.visible)
        {
            this.removeCommand?.Undo();
            return;
        }

        CreateRemoveCommand();
        this.removeCommand.Execute();
    }

    public void EvaluatePolygonVisual()
    {
        Point? point1 = this.projectRef.levels[this.levelId].GetPointById(this.point1Id);
        Point? point2 = this.projectRef.levels[this.levelId].GetPointById(this.point2Id);
        if (point1 == null || point2 == null)
            return;
        
        Vector2 point1Pos = point1.position;
        Vector2 point2Pos = point2.position;

        float distance = Vector2.GetDistance(point1Pos, point2Pos);
        Vector2 direction = Vector2.GetDirection(point1Pos, point2Pos);
        this.midPoint = point1Pos + direction * (distance / 2f);
        this.position = this.midPoint;
        
        ICollection<System.Numerics.Vector2> polygonPoints = new List<System.Numerics.Vector2>
        {
            new (point1Pos.x - this.midPoint.x, point1Pos.y - this.midPoint.y),
            new (point2Pos.x - this.midPoint.x, point2Pos.y - this.midPoint.y)
        };
        
        this.icon?.SetPoints(polygonPoints);
        this.selectedIcon?.SetPoints(polygonPoints);
        
        Vector2 screenSize = Program.instance.GetWindowSize();
        UpdateVisualPosition(screenSize);
    }

    private void CreateRemoveCommand() => this.removeCommand ??= new RemoveLineCmd(Project.instance, this);
    
    public override string ToString() =>
        $"Line [position: {this.position}, firstPnt: {this.Id}, SecondPnt: {this.IdOther}, levelId: {this.levelId}]";

    public Point[] GetPoints()
    {
        Point? point1 = this.projectRef.levels[this.levelId].GetPointById(this.point1Id);
        Point? point2 = this.projectRef.levels[this.levelId].GetPointById(this.point2Id);
        if (point1 != null && point2 != null)
            return
            [
                point1,
                point2
            ];
        return [];
    }
}