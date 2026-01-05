using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public abstract class BasePlacerTool(WindowInstance windowRef) : ITool
{
    private bool ableToPlace = true;
    protected Vector2 lastMousePosition = Vector2.Zero;
    protected Action? toCall;
    
    public void Call(MouseState mouse)
    {
        if (!ableToPlace || toCall is null) return;
        
        lastMousePosition = new Vector2(mouse.X, mouse.Y);
        var program = Program.instance;
        if (program.IsFocused() && program.IsInsideWindowBounds(lastMousePosition) &&
            !program.WasMouseClickConsumedByGum())
        {
            lastMousePosition -= new Vector2(windowRef.GetWindowWidth(), windowRef.GetWindowHeight()) / 2;
            toCall.Invoke();
        }
        
        ableToPlace = false;
    }

    public void UnEquip()
    {
        ableToPlace = true;
    }

    public void Update(float dt, WindowInstance windowRef)
    {
        var mouse = windowRef.Mouse.currentMouseState;
        if (!ableToPlace && mouse.LeftButton == ButtonState.Released)
            ableToPlace = true;
    }

    public abstract void SetVisuals();
}