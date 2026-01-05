using Gum.Wireframe;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public class DragSelect : IBaseUpdatable
{
    private Vector2? firstMousePos;
    private SelectionBox selectionBox = new();
    private RectangleRuntime visual = new();
    private Level? levelRef;
    private List<Level.Object> selectedObjects = [];

    public DragSelect()
    {
        visual.LineWidth = UIParams.defaultOutLineWidth;
        visual.IsDotted = true;
        visual.Color = UIParams.selectionColor;
        visual.Width = 0;
        visual.Height = 0;
        visual.Anchor(Anchor.Center);
        visual.AddToRoot();
        
        rightClickManager.instance.AddOptions<DragSelect>([
            new rightClickManager.RightClickOption(
                "Remove", 
                () => Program.instance.cmdHistory.ApplyCmd(new RemovePointsCmd(Project.instance, GetSelectedObjects<Point>))),
            // new rightClickManager.RightClickOption(
            //     "Connect points", 
            //     () => Program.instance.cmdHistory.ApplyCmd(new AddLinesCmd(Project.instance))
            // )
        ]);
    }

    /// <summary>
    /// Gets an array of your desired type from the drag selection, it removes them from the selection also
    /// </summary>
    /// <typeparam name="T">Level.Object</typeparam>
    /// <returns>list of objects in current drag selection</returns>
    public T[] GetSelectedObjects<T>() where T : Level.Object
    {
        T[] result = selectedObjects.OfType<T>().ToArray();
        
        var newSelection = selectedObjects.ToList();
        foreach (var obj in selectedObjects.Where(obj => result.Contains(obj))) newSelection.Remove(obj);
        selectedObjects = newSelection;
        
        if (selectedObjects.Count == 0) 
            UnSelect();
        
        return result;
    }

    /// <summary>
    /// updates the list of current selection, and visuals, as well as drag position
    /// </summary>
    /// <param name="mouse">Current mouse state</param>
    /// <returns>if user is currently drag selecting</returns>
    public bool UpdateDrag(MouseState mouse, WindowInstance windowRef)
    {
        if (Project.instance.levels.Count == 0 || windowRef.Mouse.IsDragging)
            return false;
        var lastMousePos = new Vector2(mouse.Position) - new Vector2(windowRef.GetWindowWidth() / 2, windowRef.GetWindowHeight() / 2);
        
        levelRef ??= Project.instance.levels[Project.instance.currentLevel];
        firstMousePos ??= new Vector2(lastMousePos);
        
        float width = Math.Abs(firstMousePos.x - lastMousePos.x);
        float height = Math.Abs(firstMousePos.y - lastMousePos.y);
        
        var direction = (lastMousePos - firstMousePos).Normalized;
        selectionBox.center = firstMousePos + new Vector2(direction.x * width / 2, direction.y * height / 2);
        selectionBox.size = new Vector2(width, height);

        visual.Width = width;
        visual.Height = height;
        visual.X = selectionBox.center.x;
        visual.Y = selectionBox.center.y;

        visual.Visible = selectionBox.size.Magnitude > 10;

        foreach (var obj in levelRef.levelObjects)
        {
            var isSelected = selectionBox.IsInsideBounds(obj.Position);
            if (selectedObjects.Contains(obj))
            {
                if (isSelected) continue;
                selectedObjects.Remove(obj);
                obj.HideSelectionVisual();
            }
            else if (isSelected)
            {
                selectedObjects.Add(obj);
                obj.ShowSelectionVisual();
            }
        }
        
        return visual.Visible;
    }

    public void Reset()
    {
        levelRef = null;
        firstMousePos = null;
    }

    public void UnSelect()
    {
        Reset();
        foreach (var obj in selectedObjects)
        {
            if (obj is not null)
                obj.HideSelectionVisual();
        }
        selectedObjects.Clear();

        visual.Visible = false;
    }
    
    public void Update(float dt, WindowInstance windowRef)
    {
        var mouse = windowRef.Mouse.currentMouseState;
        if (selectedObjects.Count == 0) return;
        var mousePos = new Vector2(mouse.Position) -
                       new Vector2(windowRef.GetWindowWidth(), windowRef.GetWindowHeight()) / 2;
        if (mouse.RightButton == ButtonState.Pressed && selectionBox.IsInsideBounds(mousePos))
        {
            mousePos = new Vector2(mouse.Position);
            rightClickManager.instance.ShowOptions<DragSelect>(mousePos, this, 2);
        }
    }
}