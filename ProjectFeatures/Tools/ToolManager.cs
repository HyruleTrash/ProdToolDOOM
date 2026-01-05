using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public class ToolManager : IBaseUpdatable
{
    public ITool? CurrentTool { get; set; }
    public static Dictionary<Type, ITool> tools;
    private bool wasPressed = false;

    public ToolManager(WindowInstance windowRef)
    {
        tools = new Dictionary<Type, ITool>
        {
            {typeof(EntityPlacerTool), new EntityPlacerTool(windowRef)},
            {typeof(PointPlacerTool), new PointPlacerTool(windowRef)}
        };
    }

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

    public void Update(float dt, WindowInstance windowRef)
    {
        var mouse = windowRef.Mouse.currentMouseState;
        
        var released = mouse.LeftButton == ButtonState.Released;
        var pressed = mouse.LeftButton == ButtonState.Pressed;
        
        if (!wasPressed && pressed)
            wasPressed = true;

        if (wasPressed && !windowRef.Mouse.isDragSelecting)
        {
            if (CurrentTool is not null && released)
            {
                CurrentTool?.Call(mouse);
                wasPressed = false;
                windowRef.Mouse.dragSelect?.Reset();
            }
        }

        CurrentTool?.Update(dt, windowRef);
    }
}