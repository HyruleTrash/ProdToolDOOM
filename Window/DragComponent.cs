using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;

namespace ProdToolDOOM.Window;

public class DragComponent : IHoverable
{
    private GameWindow window;
    private SelectionBox selectionBox;
    private ColoredRectangleRuntime? rectangleVisual;
    private RectangleRuntime? lineRectangleVisual;
    private const float height = UIParams.minBoxSize;
    private bool dragging = false;
    
    private Vector2 firstMousePos;
    private Vector2 lastMousePos;
    private Vector2 firstWindowPos;

    public DragComponent(Vector2 windowSize, GameWindow window)
    {
        UpdateSize(windowSize);
        this.window = window;
    }
    
    public void UpdateSize(Vector2 windowSize)
    {
        this.selectionBox = new SelectionBox(new Vector2(windowSize.x / 2, (float)height / 2), windowSize.x, height);
        if (this.rectangleVisual == null || this.lineRectangleVisual == null)
            return;
        this.rectangleVisual.Width = this.selectionBox.size.x;
        this.lineRectangleVisual.Width = this.selectionBox.size.x;
    }

    public bool CheckHover(MouseState mouseState, float dt)
    {
        bool mouseHeld = mouseState.LeftButton == ButtonState.Pressed;
        Vector2 mousePos = new(mouseState.Position);
        if ((mouseHeld && this.dragging) || (mouseHeld && this.selectionBox.IsInsideBounds(mousePos)))
        {
            Vector2 mouseScreen = mousePos + new Vector2(this.window.Position.X, this.window.Position.Y) - new Vector2((float)this.window.ClientBounds.Width / 2, (float)this.window.ClientBounds.Height / 2);
            Drag(mouseScreen, dt);
            return true;
        }

        this.dragging = false;
        return false;
    }

    private void Drag(Vector2 mousePos, float dt)
    {
        if (!this.dragging)
        {
            this.dragging = true;
            this.firstMousePos = mousePos;
            this.lastMousePos = mousePos;
            this.firstWindowPos = new Vector2(this.window.Position.X, this.window.Position.Y);
            return;
        }
        Program.instance.Mouse.SetVisual(MouseCursor.Hand, 10);
        if (mousePos == this.lastMousePos)
            return;
        
        Vector2 delta = mousePos - this.firstMousePos;

        if (delta.Magnitude >= 1)
        {
            Vector2 newPos = this.firstWindowPos + delta;
            this.window.Position = new Microsoft.Xna.Framework.Point((int)Math.Round(newPos.x), (int)Math.Round(newPos.y));
        }

        this.lastMousePos = mousePos;
    }

    public void LoadUI()
    {
        this.rectangleVisual = new ColoredRectangleRuntime
        {
            Width = this.selectionBox.size.x,
            Height = height,
            Color = UIParams.defaultFillColor,
            X = this.selectionBox.center.x
        };
        this.rectangleVisual.Anchor(Anchor.Top);

        this.lineRectangleVisual = new RectangleRuntime
        {
            Width = this.selectionBox.size.x,
            Height = height,
            Color = UIParams.defaultOutlineColor,
            X = this.selectionBox.center.x,
            LineWidth = UIParams.defaultOutLineWidth
        };
        this.lineRectangleVisual.Anchor(Anchor.Top);
        this.rectangleVisual.AddChild(this.lineRectangleVisual);
    }
    
    public void FinalizeUI()
    {
        this.rectangleVisual.AddToRoot();
    }
}