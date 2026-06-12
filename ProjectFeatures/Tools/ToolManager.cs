using System;
using System.Collections.Generic;
using DLLevelBuilder.Window;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace DLLevelBuilder.ProjectFeatures.Tools;

public class ToolManager : IBaseUpdatable
{
    private static ToolManager? instance;
    public static ToolManager Instance
    {
        get { return instance ??= new ToolManager(); }
        private set => instance = value;
    }

    private static readonly Dictionary<Type, ITool> Tools = new()
    {
        { typeof(PointPlacerTool), new PointPlacerTool() },
        { typeof(EntityPlacerTool), new EntityPlacerTool() },
    };
    private static ITool? CurrentTool { get; set; }
    private static bool wasPressed;

    private static void SetTool(ITool tool)
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
    public static void SetTool(Type tool)
    {
        if (!Tools.TryGetValue(tool, out ITool? foundTool)) return;
        SetTool(foundTool);
    }
    
    public static ITool? GetTool(Type tool) => Tools.GetValueOrDefault(tool);

    public void Update(float dt, WindowInstance windowRef)
    {
        MouseState mouse = windowRef.Mouse.currentMouseState;
        
        bool released = mouse.LeftButton == ButtonState.Released;
        bool pressed = mouse.LeftButton == ButtonState.Pressed;
        
        if (!wasPressed && pressed) wasPressed = true;

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