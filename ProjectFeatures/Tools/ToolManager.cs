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
        this.CurrentTool?.UnEquip();
        if (this.CurrentTool is not null && this.CurrentTool == tool)
        {
            this.CurrentTool = null;
            return;
        }

        this.CurrentTool = tool;
        tool.SetVisuals();
    }
    
    public void SetTool(Type tool)
    {
        if (!tools.TryGetValue(tool, out ITool? foundTool)) return;
        SetTool(foundTool);
    }

    public void Update(float dt, WindowInstance windowRef)
    {
        MouseState mouse = windowRef.Mouse.currentMouseState;
        
        bool released = mouse.LeftButton == ButtonState.Released;
        bool pressed = mouse.LeftButton == ButtonState.Pressed;
        
        if (!this.wasPressed && pressed) this.wasPressed = true;

        if (this.wasPressed && !windowRef.Mouse.isDragSelecting)
        {
            if (this.CurrentTool is not null && released)
            {
                this.CurrentTool?.Call(mouse);
                this.wasPressed = false;
                windowRef.Mouse.dragSelect?.Reset();
            }
        }

        this.CurrentTool?.Update(dt, windowRef);
    }
}