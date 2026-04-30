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
        get => this.isDragging;
        set
        {
            this.isDragging = value;
            if (value) this.dragSelect?.UnSelect();
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
        MouseCursor? mouseVisual = GetVisual();
        if (mouseVisual != null)
            Microsoft.Xna.Framework.Input.Mouse.SetCursor(mouseVisual);
        this.visualSetCalls = [new MouseVisualSetCall(MouseCursor.Arrow, 0)];
    }

    public void SetVisual(MouseCursor visualType, int priority)
    {
        this.visualSetCalls.Add(new MouseVisualSetCall(visualType, priority));
    }

    private MouseCursor? GetVisual()
    {
        MouseVisualSetCall? currentHighestPriorityCall = null;
        foreach (MouseVisualSetCall mouseVisualSetCall in this.visualSetCalls)
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

    public Vector2 GetMousePosition() => new Vector2(this.currentMouseState.Position) -
                                         new Vector2(this.windowRef.GetWindowWidth(), this.windowRef.GetWindowHeight()) / 2;

    public void Update(float dt, WindowInstance _)
    {
        this.dragSelect ??= new DragSelect();
        
        bool pressed = this.currentMouseState.LeftButton == ButtonState.Pressed;
        bool released = this.currentMouseState.LeftButton == ButtonState.Released;
        
        if (pressed && !this.windowRef.WasMouseClickConsumedByGum())
        {
            if (this.shouldResetDrag)
            {
                this.dragSelect.Reset();
                this.shouldResetDrag = false;
            }

            this.isDragSelecting = this.dragSelect.UpdateDrag(this.currentMouseState, this.windowRef);
        }
        if (released) this.shouldResetDrag = true;

        this.dragSelect.Update(dt, this.windowRef);
    }
}