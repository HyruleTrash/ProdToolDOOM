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
    
    private const float speed = 50;

    public DragComponent(Vector2 windowSize, GameWindow window)
    {
        Update(windowSize);
        this.window = window;
    }
    
    public void Update(Vector2 windowSize)
    {
        selectionBox = new SelectionBox(new Vector2(windowSize.x / 2, (float)height / 2), windowSize.x, height);
        if (rectangleVisual == null || lineRectangleVisual == null)
            return;
        rectangleVisual.Width = selectionBox.size.x;
        lineRectangleVisual.Width = selectionBox.size.x;
    }

    public bool CheckHover(MouseState mouseState, float dt)
    {
        var mouseHeld = mouseState.LeftButton == ButtonState.Pressed;
        var mousePos = new Vector2(mouseState.Position);
        if ((mouseHeld && dragging) || (mouseHeld && selectionBox.IsInsideBounds(mousePos)))
        {
            Vector2 mouseScreen = mousePos + new Vector2(window.Position.X, window.Position.Y) - new Vector2((float)window.ClientBounds.Width / 2, (float)window.ClientBounds.Height / 2);
            Drag(mouseScreen, dt);
            return true;
        }
        
        dragging = false;
        return false;
    }

    private void Drag(Vector2 mousePos, float dt)
    {
        if (!dragging)
        {
            dragging = true;
            firstMousePos = mousePos;
            lastMousePos = mousePos;
            firstWindowPos = new Vector2(window.Position.X, window.Position.Y);
            return;
        }
        Program.instance.mouse.SetVisual(MouseCursor.Hand, 10);
        if (mousePos == lastMousePos)
            return;
        
        var delta = mousePos - firstMousePos;

        if (delta.Magnitude >= 1)
        {
            var newPos = firstWindowPos + delta;
            window.Position = new Microsoft.Xna.Framework.Point((int)Math.Round(newPos.x), (int)Math.Round(newPos.y));
        }
        
        lastMousePos = mousePos;
    }

    public void LoadUI()
    {
        rectangleVisual = new ColoredRectangleRuntime
        {
            Width = selectionBox.size.x,
            Height = height,
            Color = UIParams.defaultFillColor,
            X = selectionBox.center.x
        };
        rectangleVisual.Anchor(Anchor.Top);
        rectangleVisual.AddToRoot();
        
        lineRectangleVisual = new RectangleRuntime
        {
            Width = selectionBox.size.x,
            Height = height,
            Color = UIParams.defaultOutlineColor,
            X = selectionBox.center.x,
            LineWidth = UIParams.defaultLineWidth
        };
        lineRectangleVisual.Anchor(Anchor.Top);
        rectangleVisual.AddChild(lineRectangleVisual);
    }
}