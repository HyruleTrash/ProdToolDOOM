using System;
using System.Collections.Generic;
using System.Linq;
using DLLevelBuilder.UI;
using DLLevelBuilder.Window;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace DLLevelBuilder.ProjectFeatures.Tools;

public class DragSelect : IBaseUpdatable
{
    private Vector2? firstMousePos;
    private SelectionBox selectionBox = new();
    private RectangleRuntime visual = new();
    private Level? levelRef;
    private List<LevelObject> selectedObjects = [];

    public DragSelect()
    {
        this.visual.LineWidth = Params.defaultOutLineWidth;
        this.visual.IsDotted = true;
        this.visual.Color = Params.SelectionColor;
        this.visual.Width = 0;
        this.visual.Height = 0;
        this.visual.Anchor(Anchor.Center);
        this.visual.AddToRoot();
        
        RightClickManager.instance.AddOptions<DragSelect>([
            new RightClickManager.RightClickOption(
                "Remove all", 
                () => Program.instance.cmdHistory.ApplyCmd(new RemoveLevelObjectsCmd(Project.Instance, GetSelectedObjects<LevelObject>)),
                () => this.selectedObjects.Count > 1),
            new RightClickManager.RightClickOption(
                "Remove points", 
                () => Program.instance.cmdHistory.ApplyCmd(new RemoveLevelObjectArrayCmd<Point, RemovePointCmd>(
                    Project.Instance, GetSelectedObjects<Point>, (prj, obj) => new RemovePointCmd(prj, obj))),
                () => this.selectedObjects.OfType<Point>().Any()),
            new RightClickManager.RightClickOption(
                "Remove lines", 
                () => Program.instance.cmdHistory.ApplyCmd(new RemoveLevelObjectArrayCmd<Line, RemoveLineCmd>(
                    Project.Instance, GetSelectedObjects<Line>, (prj, obj) => new RemoveLineCmd(prj, obj))),
                () => this.selectedObjects.OfType<Line>().Any()),
            new RightClickManager.RightClickOption(
                "Remove entities", 
                () => Program.instance.cmdHistory.ApplyCmd(new RemoveLevelObjectArrayCmd<Entity, RemoveEntityCmd>(
                    Project.Instance, GetSelectedObjects<Entity>, (prj, obj) => new RemoveEntityCmd(prj, obj))),
                () => this.selectedObjects.OfType<Line>().Any()),
            new RightClickManager.RightClickOption(
                "Connect points", 
                () => Program.instance.cmdHistory.ApplyCmd(new AddLinesCmd(Project.Instance, Program.instance, GetSelectedObjects<Point>)),
                () =>
                {
                    bool hadOne = false;
                    foreach (Point? point in this.selectedObjects.Select(selectedObject => selectedObject as Point)) // Checks if there's atleast more then 1 point
                    {
                        if (point != null && hadOne)
                            return true;
                        if (point != null && !hadOne) 
                            hadOne = true;
                    }
                    return false;
                })
        ]);
    }

    /// <summary>
    /// Gets an array of your desired type from the drag selection, it removes them from the selection also
    /// </summary>
    /// <typeparam name="T">Object</typeparam>
    /// <returns>list of objects in current drag selection</returns>
    public T[] GetSelectedObjects<T>(bool shouldRemove = false) where T : LevelObject
    {
        T[] result = this.selectedObjects.OfType<T>().ToArray();
        
        if (!shouldRemove)
            return result;
        List<LevelObject> newSelection = this.selectedObjects.ToList();
        foreach (LevelObject obj in this.selectedObjects.Where(obj => result.Contains(obj))) newSelection.Remove(obj);
        this.selectedObjects = newSelection;
        
        if (this.selectedObjects.Count == 0) 
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
        if (Project.Instance.levels.Count == 0 || windowRef.Mouse.IsDragging)
            return false;
        Vector2 lastMousePos = new Vector2(mouse.Position) - new Vector2(windowRef.GetWindowWidth() / 2, windowRef.GetWindowHeight() / 2);

        this.levelRef ??= Project.TryGetCurrentLevel();
        if (this.levelRef == null) return false;
        this.firstMousePos ??= new Vector2(lastMousePos);
        
        float width = Math.Abs(this.firstMousePos.x - lastMousePos.x);
        float height = Math.Abs(this.firstMousePos.y - lastMousePos.y);

        this.selectionBox.center = (this.firstMousePos + lastMousePos) / 2f;
        this.selectionBox.size = new Vector2(width, height);

        this.visual.Width = width;
        this.visual.Height = height;
        this.visual.X = this.selectionBox.center.x;
        this.visual.Y = this.selectionBox.center.y;

        this.visual.Visible = this.selectionBox.size.Magnitude > 10;

        foreach (var obj in this.levelRef.levelObjects)
        {
            if (obj is null || obj.position is null)
                continue;
            bool isSelected = this.selectionBox.IsInsideBounds(obj.position);
            if (this.selectedObjects.Contains(obj))
            {
                if (isSelected) continue;
                this.selectedObjects.Remove(obj);
                obj.HideSelectionVisual();
            }
            else if (isSelected)
            {
                this.selectedObjects.Add(obj);
                obj.ShowSelectionVisual();
            }
        }
        
        return this.visual.Visible;
    }

    public void Reset()
    {
        this.levelRef = null;
        this.firstMousePos = null;
    }

    public void UnSelect()
    {
        Reset();
        foreach (var obj in this.selectedObjects)
        {
            if (obj is not null)
                obj.HideSelectionVisual();
        }

        this.selectedObjects.Clear();
        this.visual.Visible = false;
        RightClickManager.instance.HideOptions<DragSelect>();
    }
    
    public void Update(float dt, WindowInstance windowRef)
    {
        MouseState mouse = windowRef.Mouse.currentMouseState;
        if (this.selectedObjects.Count == 0) return;
        Vector2 mousePos = new Vector2(mouse.Position) -
                           new Vector2(windowRef.GetWindowWidth(), windowRef.GetWindowHeight()) / 2;
        if (mouse.RightButton == ButtonState.Pressed && this.selectionBox.IsInsideBounds(mousePos))
        {
            mousePos = new Vector2(mouse.Position);
            RightClickManager.instance.ShowOptions<DragSelect>(mousePos, this, 2);
        }
    }
}