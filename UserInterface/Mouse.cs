using Microsoft.Xna.Framework.Input;
using ProdToolDOOM.ProjectFeatures.Tools;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM;

public struct MouseVisualSetCall(MouseCursor givenType, int givenPriority)
{
    public readonly MouseCursor type = givenType;
    public readonly int priority = givenPriority;
}

public class Mouse(WindowInstance windowRef) : IBaseUpdatable
{
    public MouseState currentMouseState;

    public bool IsDragging
    {
        get => isDragging;
        set
        {
            isDragging = value;
            if (value)
                dragSelect?.UnSelect();
        }
    }

    private bool isDragging = false;
    public bool isDragSelecting;
    
    public DragSelect? dragSelect;
    private bool shouldResetDrag;
    
    private List<MouseVisualSetCall> visualSetCalls = [];
    private WindowInstance windowRef = windowRef;
    
    public void UpdateVisual()
    {
        var mouseVisual = GetVisual();
        if (mouseVisual != null)
            Microsoft.Xna.Framework.Input.Mouse.SetCursor(mouseVisual);
        visualSetCalls = [new MouseVisualSetCall(MouseCursor.Arrow, 0)];
    }

    public void SetVisual(MouseCursor visualType, int priority)
    {
        visualSetCalls.Add(new MouseVisualSetCall(visualType, priority));
    }

    private MouseCursor? GetVisual()
    {
        MouseVisualSetCall? currentHighestPriorityCall = null;
        foreach (var mouseVisualSetCall in visualSetCalls)
        {
            if (currentHighestPriorityCall == null)
            {
                currentHighestPriorityCall = mouseVisualSetCall;
                continue;
            }
            if (currentHighestPriorityCall.Value.priority < mouseVisualSetCall.priority)
            {
                currentHighestPriorityCall = mouseVisualSetCall;
            }
        }

        return currentHighestPriorityCall?.type;
    }

    public Vector2 GetMousePosition() => new Vector2(currentMouseState.Position) -
                                         new Vector2(windowRef.GetWindowWidth(), windowRef.GetWindowHeight()) / 2;

    public void Update(float dt, WindowInstance _)
    {
        dragSelect ??= new DragSelect();
        
        var pressed = currentMouseState.LeftButton == ButtonState.Pressed;
        var released = currentMouseState.LeftButton == ButtonState.Released;
        
        if (pressed && !windowRef.WasMouseClickConsumedByGum())
        {
            if (shouldResetDrag)
            {
                dragSelect.Reset();
                shouldResetDrag = false;
            }
            isDragSelecting = dragSelect.UpdateDrag(currentMouseState, windowRef);
        }
        if (released)
            shouldResetDrag = true;
        
        dragSelect.Update(dt, windowRef);
    }
}