using Microsoft.Xna.Framework.Input;

namespace ProdToolDOOM;

public struct MouseVisualSetCall(MouseCursor givenType, int givenPriority)
{
    public MouseCursor type = givenType;
    public int priority = givenPriority;
}