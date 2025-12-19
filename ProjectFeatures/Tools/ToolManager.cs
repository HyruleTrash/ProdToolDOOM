using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public class ToolManager
{
    public ITool? CurrentTool { get; set; }
    public static Dictionary<Type, ITool> tools = new()
    {
        {typeof(PointPlacerTool), new PointPlacerTool()}
    };

    public void SetTool(ITool tool)
    {
        CurrentTool?.UnEquip();
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
        if (mouse.LeftButton == ButtonState.Pressed)
            CurrentTool?.Call(mouse);
        CurrentTool?.Update(dt, mouse);
    }
}