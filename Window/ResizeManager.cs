using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Point = Microsoft.Xna.Framework.Point;

namespace ProdToolDOOM.Window;

public class ResizeManager
{
    public abstract class Side(SelectionBox box)
    {
        public bool currentlyHoveredOver = false;
        public bool isBeingResized = false;
        protected SelectionBox sideSelectionBox = box;
        protected int savedOldPos;
        protected float mouseOffset;

        public void UpdateCurrentlyHoveredOver(Vector2 mousePos) => currentlyHoveredOver = sideSelectionBox.IsInsideBounds(mousePos);

        public abstract void SetMouseVisual();
        protected abstract float GetEdgePosition();
        protected abstract void SavePosValueInMemory(GameWindow window);
        protected abstract float CalculateMouseOffset(float edgePos, Vector2 mouse);
        protected abstract int GetPosWithinWindowBounds(GraphicsDeviceManager graphics, float pos);
        protected abstract void ApplyPreferredBuffer(GraphicsDeviceManager graphics);
        
        public virtual void ResizeSide(GraphicsDeviceManager graphics, GameWindow window, Vector2 mousePos)
        {
            SavePosValueInMemory(window);
            mouseOffset = CalculateMouseOffset(GetEdgePosition(), mousePos);
        }
    }

    private abstract class SideX(SelectionBox box) : Side(box)
    {
        public override void SetMouseVisual()
        {
            Program.instance.SetMouseVisual(MouseCursor.SizeWE, 10);
        }

        protected override float GetEdgePosition()
        {
            return sideSelectionBox.center.x;
        }

        protected override void SavePosValueInMemory(GameWindow window)
        {
            savedOldPos = window.Position.X;
        }

        protected override int GetPosWithinWindowBounds(GraphicsDeviceManager graphics, float pos)
        {
            return Math.Min(Math.Max(graphics.PreferredBackBufferHeight + (int)Math.Ceiling(pos), UIParams.minWindowWidth), 10000);
        }

        protected override void ApplyPreferredBuffer(GraphicsDeviceManager graphics)
        {
            graphics.PreferredBackBufferWidth = GetPosWithinWindowBounds(graphics, mouseOffset / 2);
            graphics.ApplyChanges();
        }
    }

    private class RightSide(SelectionBox box) : SideX(box)
    {
        protected override float CalculateMouseOffset(float edgePos, Vector2 mouse)
        {
            return mouse.x - edgePos;
        }

        public override void ResizeSide(GraphicsDeviceManager graphics, GameWindow window, Vector2 mousePos)
        {
            base.ResizeSide(graphics, window, mousePos);

            ApplyPreferredBuffer(graphics);
            
            if (window.Position.X != savedOldPos)
                window.Position = new Point(savedOldPos, window.Position.Y);
        }
    }

    private class LeftSide(SelectionBox box) : SideX(box)
    {
        protected override float CalculateMouseOffset(float edgePos, Vector2 mouse)
        {
            return edgePos - mouse.x;
        }

        public override void ResizeSide(GraphicsDeviceManager graphics, GameWindow window, Vector2 mousePos)
        {
            base.ResizeSide(graphics, window, mousePos);
            
            window.Position = new Point(savedOldPos - (int)Math.Ceiling(mouseOffset / 2), window.Position.Y);
            
            ApplyPreferredBuffer(graphics);
        }
    }
    
    private abstract class SideY(SelectionBox box) : Side(box)
    {
        public override void SetMouseVisual()
        {
            Program.instance.SetMouseVisual(MouseCursor.SizeNS, 10);
        }

        protected override float GetEdgePosition()
        {
            return sideSelectionBox.center.y;
        }

        protected override void SavePosValueInMemory(GameWindow window)
        {
            savedOldPos = window.Position.Y;
        }
        
        protected override int GetPosWithinWindowBounds(GraphicsDeviceManager graphics, float pos)
        {
            return Math.Min(Math.Max(graphics.PreferredBackBufferHeight + (int)Math.Ceiling(pos), UIParams.minWindowHeight), 10000);
        }

        protected override void ApplyPreferredBuffer(GraphicsDeviceManager graphics)
        {
            graphics.PreferredBackBufferHeight = GetPosWithinWindowBounds(graphics, mouseOffset / 2);
            graphics.ApplyChanges();
        }
    }

    private class BottomSide(SelectionBox box) : SideY(box)
    {
        protected override float CalculateMouseOffset(float edgePos, Vector2 mouse)
        {
            return mouse.y - edgePos;
        }

        public override void ResizeSide(GraphicsDeviceManager graphics, GameWindow window, Vector2 mousePos)
        {
            base.ResizeSide(graphics, window, mousePos);

            graphics.PreferredBackBufferHeight = Math.Max(
                UIParams.minWindowHeight,
                graphics.PreferredBackBufferHeight + (int)Math.Ceiling(mouseOffset / 2));
            graphics.ApplyChanges();
            
            if (window.Position.Y != savedOldPos)
                window.Position = new Point(window.Position.X, savedOldPos);
        }
    }

    private class TopSide(SelectionBox box) : SideY(box)
    {
        protected override float CalculateMouseOffset(float edgePos, Vector2 mouse)
        {
            return edgePos - mouse.y;
        }

        public override void ResizeSide(GraphicsDeviceManager graphics, GameWindow window, Vector2 mousePos)
        {
            base.ResizeSide(graphics, window, mousePos);

            window.Position = new Point(window.Position.X, savedOldPos - (int)Math.Ceiling(mouseOffset / 2));
            
            graphics.PreferredBackBufferHeight = Math.Max(
                UIParams.minWindowHeight,
                graphics.PreferredBackBufferHeight + (int)Math.Ceiling(mouseOffset / 2));
            graphics.ApplyChanges();
        }
    }
    
    private class Sides(SelectionBox rightSide, SelectionBox leftSide, SelectionBox topSide, SelectionBox bottomSide)
    {
        public RightSide rightSide = new(rightSide);
        public LeftSide leftSide = new(leftSide);
        public TopSide topSide = new(topSide);
        public BottomSide bottomSide = new(bottomSide);
        
        public bool GetHoveredOverX() => rightSide.currentlyHoveredOver || leftSide.currentlyHoveredOver;
        public bool GetHoveredOverY() => topSide.currentlyHoveredOver || bottomSide.currentlyHoveredOver;
        public void CheckHover(ResizeManager manager)
        {
            void Check(Side side)
            {
                if (!side.currentlyHoveredOver) return;
                manager.TriggerResize(side);
                side.isBeingResized = true;
            }
            
            Check(rightSide);
            Check(leftSide);
            Check(topSide);
            Check(bottomSide);
        }
        public void StopAnyResizing()
        {
            Side[] sides = [rightSide, leftSide, topSide, bottomSide];
            foreach (var s in sides)
            {
                s.isBeingResized = false;
            }
        }
    }
    
    private Sides resizeBoxes;
    private bool isResizing;
    private Side currentSideToResize = null!;

    public ResizeManager(Vector2 windowSize)
    {
        ResizeSelectionBoxData(windowSize);
    }
    
    public void ResizeSelectionBoxData(Vector2 windowSize)
    {
        resizeBoxes = new Sides(
            new SelectionBox(new Vector2(windowSize.x, windowSize.y / 2), UIParams.minNearSelection, windowSize.y),
            new SelectionBox(new Vector2(0, windowSize.y / 2), UIParams.minNearSelection, windowSize.y),
            new SelectionBox(new Vector2(windowSize.x / 2, 0), windowSize.x, UIParams.minNearSelection),
            new SelectionBox(new Vector2(windowSize.x / 2, windowSize.y), windowSize.x, UIParams.minNearSelection)
        );
    }

    public void CheckResizePositions(Vector2 mousePosition, MouseState mouseState)
    {
        var mouseHeld = mouseState.LeftButton == ButtonState.Pressed;

        resizeBoxes.rightSide.UpdateCurrentlyHoveredOver(mousePosition);
        resizeBoxes.leftSide.UpdateCurrentlyHoveredOver(mousePosition);
        resizeBoxes.topSide.UpdateCurrentlyHoveredOver(mousePosition);
        resizeBoxes.bottomSide.UpdateCurrentlyHoveredOver(mousePosition);

        if (resizeBoxes.GetHoveredOverX())
            Program.instance.SetMouseVisual(MouseCursor.SizeWE, 1);
        else if (resizeBoxes.GetHoveredOverY())
            Program.instance.SetMouseVisual(MouseCursor.SizeNS, 1);

        if (mouseHeld)
            resizeBoxes.CheckHover(this);
        else{
            resizeBoxes.StopAnyResizing();
            isResizing = false;
        }
    }
    
    public void TriggerResize(Side side)
    {
        isResizing = true;
        currentSideToResize = side;
    }

    public void ResizeWindow(GraphicsDeviceManager graphics, GameWindow window)
    {
        if (Program.instance.Fullscreen || !isResizing)
            return;

        currentSideToResize.SetMouseVisual();

        var mouse = Mouse.GetState();
        currentSideToResize.ResizeSide(graphics, window, new Vector2(mouse.X, mouse.Y));

        Program.instance.onScreenSizeChange?.Invoke(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
    }
}