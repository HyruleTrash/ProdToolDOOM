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
        if (!this.ableToPlace || this.toCall is null) return;

        this.lastMousePosition = new Vector2(mouse.X, mouse.Y);
        Program program = Program.instance;
        if (program.IsFocused() && program.IsInsideWindowBounds(this.lastMousePosition) &&
            !program.WasMouseClickConsumedByGum())
        {
            this.lastMousePosition -= new Vector2(windowRef.GetWindowWidth(), windowRef.GetWindowHeight()) / 2;
            this.toCall.Invoke();
        }

        this.ableToPlace = false;
    }

    public void UnEquip()
    {
        this.ableToPlace = true;
    }

    public void Update(float dt, WindowInstance windowRef)
    {
        MouseState mouse = windowRef.Mouse.currentMouseState;
        if (!this.ableToPlace && mouse.LeftButton == ButtonState.Released) this.ableToPlace = true;
    }

    public abstract void SetVisuals();
}