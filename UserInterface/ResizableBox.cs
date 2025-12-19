using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM;

public class ResizableBox : IHoverable
{
    protected abstract class Side(SelectionBox box)
    {
        public bool currentlyHoveredOver;
        public bool isBeingResized;
        protected readonly SelectionBox sideSelectionBox = box;
        protected float mouseOffset;
        protected IMouseOffsetCalculator mouseOffsetCalculator = null!;

        public void UpdateCurrentlyHoveredOver(Vector2 mousePos) => currentlyHoveredOver = sideSelectionBox.IsInsideBounds(mousePos);

        public abstract void SetMouseVisual();
        protected abstract float GetEdgePosition();
        protected abstract int GetPosWithinMinimumBounds(float pos);
        protected abstract void ApplySize();
        
        public virtual void ResizeSide(Vector2 mousePos)
        {
            mouseOffset = mouseOffsetCalculator.CalculateMouseOffset(GetEdgePosition(), mousePos);
        }
    }

    protected interface IMouseOffsetCalculator
    {
        public float CalculateMouseOffset(float edgePos, Vector2 mouse);
    }

    protected abstract class SideX(SelectionBox box) : Side(box)
    {
        public override void SetMouseVisual()
        {
            Program.instance.mouse.SetVisual(MouseCursor.SizeWE, 10);
        }

        protected override float GetEdgePosition()
        {
            return sideSelectionBox.center.x;
        }
    }

    protected class RightSide : IMouseOffsetCalculator
    {
        public float CalculateMouseOffset(float edgePos, Vector2 mouse)
        {
            return mouse.x - edgePos;
        }
    }

    protected class LeftSide : IMouseOffsetCalculator
    {
        public float CalculateMouseOffset(float edgePos, Vector2 mouse)
        {
            return edgePos - mouse.x;
        }
    }

    protected abstract class SideY(SelectionBox box) : Side(box)
    {
        public override void SetMouseVisual()
        {
            Program.instance.mouse.SetVisual(MouseCursor.SizeNS, 10);
        }

        protected override float GetEdgePosition()
        {
            return sideSelectionBox.center.y;
        }
    }

    protected class BottomSide : IMouseOffsetCalculator
    {
        public float CalculateMouseOffset(float edgePos, Vector2 mouse)
        {
            return mouse.y - edgePos;
        }
    }

    protected class TopSide : IMouseOffsetCalculator
    {
        public float CalculateMouseOffset(float edgePos, Vector2 mouse)
        {
            return edgePos - mouse.y;
        }
    }

    protected class Sides(Side rightSide, Side leftSide, Side topSide, Side bottomSide)
    {
        public Side rightSide = rightSide;
        public Side leftSide = leftSide;
        public Side topSide = topSide;
        public Side bottomSide = bottomSide;
        
        public bool GetHoveredOverX() => rightSide.currentlyHoveredOver || leftSide.currentlyHoveredOver;
        public bool GetHoveredOverY() => topSide.currentlyHoveredOver || bottomSide.currentlyHoveredOver;
        public void CheckHover(ResizableBox manager)
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
    
    protected Sides resizeBoxes = null!;
    protected bool isResizing;
    protected Side? currentSideToResize;
    
    public bool CheckHover(MouseState mouseState, float dt)
    {
        CheckResizePositions(new Vector2(mouseState.X, mouseState.Y), mouseState);
        return resizeBoxes.GetHoveredOverX() || resizeBoxes.GetHoveredOverY();
    }
    
    public void CheckResizePositions(Vector2 mousePosition, MouseState mouseState)
    {
        var mouseHeld = mouseState.LeftButton == ButtonState.Pressed;

        resizeBoxes.rightSide.UpdateCurrentlyHoveredOver(mousePosition);
        resizeBoxes.leftSide.UpdateCurrentlyHoveredOver(mousePosition);
        resizeBoxes.topSide.UpdateCurrentlyHoveredOver(mousePosition);
        resizeBoxes.bottomSide.UpdateCurrentlyHoveredOver(mousePosition);

        if (resizeBoxes.GetHoveredOverX())
            Program.instance.mouse.SetVisual(MouseCursor.SizeWE, 1);
        else if (resizeBoxes.GetHoveredOverY())
            Program.instance.mouse.SetVisual(MouseCursor.SizeNS, 1);

        if (mouseHeld)
            resizeBoxes.CheckHover(this);
        else{
            resizeBoxes.StopAnyResizing();
            isResizing = false;
        }
    }
    
    protected void TriggerResize(Side side)
    {
        isResizing = true;
        currentSideToResize = side;
    }

    public virtual bool ShouldResize() => !isResizing || currentSideToResize == null;

    public virtual bool ResizeWindow()
    {
        if (ShouldResize())
            return false;

        currentSideToResize.SetMouseVisual();

        var mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
        currentSideToResize.ResizeSide(new Vector2(mouse.X, mouse.Y));
        
        return true;
    }
}