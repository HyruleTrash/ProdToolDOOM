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

        public void UpdateCurrentlyHoveredOver(Vector2 mousePos) => this.currentlyHoveredOver = this.sideSelectionBox.IsInsideBounds(mousePos);

        public abstract void SetMouseVisual();
        protected abstract float GetEdgePosition();
        protected abstract int GetPosWithinMinimumBounds(float pos);
        protected abstract void ApplySize();
        
        public virtual void ResizeSide(Vector2 mousePos)
        {
            this.mouseOffset = this.mouseOffsetCalculator.CalculateMouseOffset(GetEdgePosition(), mousePos);
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
            Program.instance.Mouse.SetVisual(MouseCursor.SizeWE, 10);
        }

        protected override float GetEdgePosition()
        {
            return this.sideSelectionBox.center.x;
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
            Program.instance.Mouse.SetVisual(MouseCursor.SizeNS, 10);
        }

        protected override float GetEdgePosition()
        {
            return this.sideSelectionBox.center.y;
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
        
        public bool GetHoveredOverX() => this.rightSide.currentlyHoveredOver || this.leftSide.currentlyHoveredOver;
        public bool GetHoveredOverY() => this.topSide.currentlyHoveredOver || this.bottomSide.currentlyHoveredOver;
        public void CheckHover(ResizableBox manager)
        {
            void Check(Side side)
            {
                if (!side.currentlyHoveredOver) return;
                manager.TriggerResize(side);
                side.isBeingResized = true;
            }
            
            Check(this.rightSide);
            Check(this.leftSide);
            Check(this.topSide);
            Check(this.bottomSide);
        }
        public void StopAnyResizing()
        {
            Side[] sides = [this.rightSide, this.leftSide, this.topSide, this.bottomSide];
            foreach (Side s in sides)
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
        return this.resizeBoxes.GetHoveredOverX() || this.resizeBoxes.GetHoveredOverY();
    }
    
    public void CheckResizePositions(Vector2 mousePosition, MouseState mouseState)
    {
        bool mouseHeld = mouseState.LeftButton == ButtonState.Pressed;

        this.resizeBoxes.rightSide.UpdateCurrentlyHoveredOver(mousePosition);
        this.resizeBoxes.leftSide.UpdateCurrentlyHoveredOver(mousePosition);
        this.resizeBoxes.topSide.UpdateCurrentlyHoveredOver(mousePosition);
        this.resizeBoxes.bottomSide.UpdateCurrentlyHoveredOver(mousePosition);

        if (this.resizeBoxes.GetHoveredOverX())
            Program.instance.Mouse.SetVisual(MouseCursor.SizeWE, 1);
        else if (this.resizeBoxes.GetHoveredOverY())
            Program.instance.Mouse.SetVisual(MouseCursor.SizeNS, 1);

        if (mouseHeld)
            this.resizeBoxes.CheckHover(this);
        else{
            this.resizeBoxes.StopAnyResizing();
            this.isResizing = false;
        }
    }
    
    protected void TriggerResize(Side side)
    {
        this.isResizing = true;
        this.currentSideToResize = side;
    }

    public virtual bool ShouldResize() => !this.isResizing || this.currentSideToResize == null;

    public virtual bool ResizeWindow()
    {
        if (ShouldResize())
            return false;

        this.currentSideToResize.SetMouseVisual();

        MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
        this.currentSideToResize.ResizeSide(new Vector2(mouse.X, mouse.Y));
        
        return true;
    }
}