using Microsoft.Xna.Framework;

namespace ProdToolDOOM.Window;
using Point = Microsoft.Xna.Framework.Point;

public class ResizeComponent : ResizableBox
{
    protected new abstract class SideX(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : ResizableBox.SideX(box)
    {
        protected GraphicsDeviceManager graphics = graphics;
        protected GameWindow window = window;

        protected override int GetPosWithinMinimumBounds(float pos)
        {
            int delta = (int)Math.Ceiling(pos);
            if (delta > UIParams.minResizePerFrame)
                delta = UIParams.minResizePerFrame;
            
            int desiredDelta = this.graphics.PreferredBackBufferWidth + delta;
            return Math.Max(desiredDelta, UIParams.minWindowWidth);
        }

        protected override void ApplySize()
        {
            this.graphics.PreferredBackBufferWidth = GetPosWithinMinimumBounds(this.mouseOffset);
            this.graphics.ApplyChanges();
        }
    }

    protected new class RightSide : SideX
    {
        public RightSide(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : base(box, graphics, window)
        {
            this.mouseOffsetCalculator = new ResizableBox.RightSide();
        }
        
        public override void ResizeSide(Vector2 mousePos)
        {
            base.ResizeSide(mousePos);

            this.window.Position = new Point(this.window.Position.X, this.window.Position.Y);

            ApplySize();

            this.sideSelectionBox.center.x += this.mouseOffset;
        }
    }
    
    protected new class LeftSide : SideX
    {
        public LeftSide(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : base(box, graphics, window)
        {
            this.mouseOffsetCalculator = new ResizableBox.LeftSide();
        }
        
        public override void ResizeSide(Vector2 mousePos)
        {
            base.ResizeSide(mousePos);

            this.window.Position = new Point(this.window.Position.X - (int)Math.Ceiling(this.mouseOffset), this.window.Position.Y);
            
            ApplySize();
        }
    }

    protected new abstract class SideY(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : ResizableBox.SideY(box)
    {
        protected GraphicsDeviceManager graphics = graphics;
        protected GameWindow window = window;
    
        protected override int GetPosWithinMinimumBounds(float pos)
        {
            int delta = (int)Math.Ceiling(pos);
            if (delta > UIParams.minResizePerFrame)
                delta = UIParams.minResizePerFrame;
            
            int desiredDelta = this.graphics.PreferredBackBufferHeight + delta;
            return Math.Max(desiredDelta, UIParams.minWindowHeight);
        }

        protected override void ApplySize()
        {
            this.graphics.PreferredBackBufferHeight = GetPosWithinMinimumBounds(this.mouseOffset);
            this.graphics.ApplyChanges();
        }
    }

    protected new class BottomSide : SideY
    {
        public BottomSide(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : base(box, graphics, window)
        {
            this.mouseOffsetCalculator = new ResizableBox.BottomSide();
        }
    
        public override void ResizeSide(Vector2 mousePos)
        {
            base.ResizeSide(mousePos);

            this.window.Position = new Point(this.window.Position.X, this.window.Position.Y);

            ApplySize();

            this.sideSelectionBox.center.y += this.mouseOffset;
        }
    }

    protected new class TopSide : SideY
    {
        public TopSide(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : base(box, graphics, window)
        {
            this.mouseOffsetCalculator = new ResizableBox.TopSide();
        }
        
        public override void ResizeSide(Vector2 mousePos)
        {
            base.ResizeSide(mousePos);

            this.window.Position = new Point(this.window.Position.X, this.window.Position.Y - (int)Math.Ceiling(this.mouseOffset));
            
            ApplySize();
        }
    }
    
    private GraphicsDeviceManager graphics;
    private GameWindow window;
    
    public ResizeComponent(Vector2 windowSize, GraphicsDeviceManager graphics, GameWindow window)
    {
        this.graphics = graphics;
        this.window = window;
        ResizeSelectionBoxData(windowSize);
    }
    
    public void ResizeSelectionBoxData(Vector2 windowSize)
    {
        this.resizeBoxes = new Sides(
            new RightSide(new SelectionBox(new Vector2(windowSize.x, windowSize.y / 2), UIParams.minNearSelection, windowSize.y), this.graphics, this.window),
            new LeftSide(new SelectionBox(new Vector2(0, windowSize.y / 2), UIParams.minNearSelection, windowSize.y), this.graphics, this.window),
            new TopSide(new SelectionBox(new Vector2(windowSize.x / 2, 0), windowSize.x, UIParams.minNearSelection), this.graphics, this.window),
            new BottomSide(new SelectionBox(new Vector2(windowSize.x / 2, windowSize.y), windowSize.x, UIParams.minNearSelection), this.graphics, this.window)
        );
    }
    
    public override bool ShouldResize() => Program.instance.Fullscreen || base.ShouldResize();

    public override bool ResizeWindow()
    {
        if (!base.ResizeWindow())
            return false;
        Vector2 windowSize = new Vector2(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
        Program.instance.onScreenSizeChange?.Invoke(windowSize);
        return true;
    }
}