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
    private Vector2? lastWindowPos;
    
    private const float speed = 20;

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
        rectangleVisual.X = selectionBox.center.x;
        lineRectangleVisual.Width = selectionBox.size.x;
        lineRectangleVisual.X = selectionBox.center.x;
    }

    public bool CheckHover(MouseState mouseState, float dt)
    {
        UpdateWindowPosition(dt);
        var mouseHeld = mouseState.LeftButton == ButtonState.Pressed;
        var mousePos = new Vector2(mouseState.Position.X, mouseState.Position.Y);
        if ((mouseHeld && dragging) || (mouseHeld && selectionBox.IsInsideBounds(mousePos)))
        {
            Drag(mousePos, dt);
            return true;
        }
        
        dragging = false;
        lastWindowPos = null;
        return false;
    }

    private void UpdateWindowPosition(float dt)
    {
        if (lastWindowPos == null)
            return;

        var currentWindowPos = new Vector2(window.Position.X, window.Position.Y);
        var difference = currentWindowPos - lastWindowPos.Value;
        Helper.SetWindowPosition(window.Handle, currentWindowPos - difference * dt * speed);
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
        
        var difference = lastMousePos - firstMousePos;

        if (difference.Magnitude >= 1)
        {
            var newWindowPos = firstWindowPos + difference;
            lastWindowPos = newWindowPos;
        }
        
        lastMousePos = mousePos;
    }

    public void LoadUI()
    {
        rectangleVisual = new ColoredRectangleRuntime
        {
            Width = selectionBox.size.x,
            Height = height,
            Color = Color.Gray,
            X = selectionBox.center.x
        };
        rectangleVisual.Anchor(Anchor.Top);
        rectangleVisual.AddToRoot();
        
        lineRectangleVisual = new RectangleRuntime
        {
            Width = selectionBox.size.x,
            Height = height,
            Color = Color.DimGray,
            X = selectionBox.center.x
        };
        lineRectangleVisual.Anchor(Anchor.Top);
        rectangleVisual.AddChild(lineRectangleVisual);
    }
}