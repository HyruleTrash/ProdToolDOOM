using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public class ToolManager
{
    public ITool? CurrentTool { get; set; }
    public static Dictionary<Type, ITool> tools = new()
    {
        {typeof(EntityPlacerTool), new EntityPlacerTool()},
        {typeof(PointPlacerTool), new PointPlacerTool()}
    };
    private bool wasPressed = false;
    private bool dragging = false;
    private bool shouldResetDrag = false;
    public DragSelect? dragSelect;

    public void SetTool(ITool tool)
    {
        CurrentTool?.UnEquip();
        if (CurrentTool is not null && CurrentTool == tool)
        {
            CurrentTool = null;
            return;
        }  
        CurrentTool = tool;
        tool.SetVisuals();
    }
    
    public void SetTool(Type tool)
    {
        if (!tools.TryGetValue(tool, out var foundTool)) return;
        SetTool(foundTool);
    }

    public void Update(MouseState mouse, float dt)
    {
        dragSelect ??= new DragSelect();
        
        var released = mouse.LeftButton == ButtonState.Released;
        var pressed = mouse.LeftButton == ButtonState.Pressed;

        if (pressed && !Program.instance.WasMouseClickConsumedByGum())
        {
            if (shouldResetDrag)
            {
                dragSelect.Reset();
                shouldResetDrag = false;
            }
            dragging = dragSelect.UpdateDrag(mouse);
        }
        if (released)
            shouldResetDrag = true;
        
        if (!wasPressed && pressed)
            wasPressed = true;

        if (wasPressed && !dragging)
        {
            if (CurrentTool is not null && released)
            {
                CurrentTool?.Call(mouse);
                wasPressed = false;
                dragSelect.Reset();
            }
        }

        CurrentTool?.Update(dt, mouse);
        dragSelect.Update(mouse);
    }
}