using Microsoft.Xna.Framework.Input;

namespace ProdToolDOOM;

public struct MouseVisualSetCall(MouseCursor givenType, int givenPriority)
{
    public readonly MouseCursor type = givenType;
    public readonly int priority = givenPriority;
}

public class Mouse
{
    private List<MouseVisualSetCall> visualSetCalls = [];
    
    public void Update()
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
}