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
            var delta = (int)Math.Ceiling(pos);
            if (delta > UIParams.minResizePerFrame)
                delta = UIParams.minResizePerFrame;
            
            var desiredDelta = graphics.PreferredBackBufferWidth + delta;
            return Math.Max(desiredDelta, UIParams.minWindowWidth);
        }

        protected override void ApplySize()
        {
            graphics.PreferredBackBufferWidth = GetPosWithinMinimumBounds(mouseOffset);
            graphics.ApplyChanges();
        }
    }

    protected new class RightSide : SideX
    {
        public RightSide(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : base(box, graphics, window)
        {
            mouseOffsetCalculator = new ResizableBox.RightSide();
        }
        
        public override void ResizeSide(Vector2 mousePos)
        {
            base.ResizeSide(mousePos);
            
            window.Position = new Point(window.Position.X, window.Position.Y);

            ApplySize();
            
            sideSelectionBox.center.x += mouseOffset;
        }
    }
    
    protected new class LeftSide : SideX
    {
        public LeftSide(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : base(box, graphics, window)
        {
            mouseOffsetCalculator = new ResizableBox.LeftSide();
        }
        
        public override void ResizeSide(Vector2 mousePos)
        {
            base.ResizeSide(mousePos);
            
            window.Position = new Point(window.Position.X - (int)Math.Ceiling(mouseOffset), window.Position.Y);
            
            ApplySize();
        }
    }

    protected new abstract class SideY(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : ResizableBox.SideY(box)
    {
        protected GraphicsDeviceManager graphics = graphics;
        protected GameWindow window = window;
    
        protected override int GetPosWithinMinimumBounds(float pos)
        {
            var delta = (int)Math.Ceiling(pos);
            if (delta > UIParams.minResizePerFrame)
                delta = UIParams.minResizePerFrame;
            
            var desiredDelta = graphics.PreferredBackBufferHeight + delta;
            return Math.Max(desiredDelta, UIParams.minWindowHeight);
        }

        protected override void ApplySize()
        {
            graphics.PreferredBackBufferHeight = GetPosWithinMinimumBounds(mouseOffset);
            graphics.ApplyChanges();
        }
    }

    protected new class BottomSide : SideY
    {
        public BottomSide(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : base(box, graphics, window)
        {
            mouseOffsetCalculator = new ResizableBox.BottomSide();
        }
    
        public override void ResizeSide(Vector2 mousePos)
        {
            base.ResizeSide(mousePos);
            
            window.Position = new Point(window.Position.X, window.Position.Y);

            ApplySize();
            
            sideSelectionBox.center.y += mouseOffset;
        }
    }

    protected new class TopSide : SideY
    {
        public TopSide(SelectionBox box, GraphicsDeviceManager graphics, GameWindow window) : base(box, graphics, window)
        {
            mouseOffsetCalculator = new ResizableBox.TopSide();
        }
        
        public override void ResizeSide(Vector2 mousePos)
        {
            base.ResizeSide(mousePos);

            window.Position = new Point(window.Position.X, window.Position.Y - (int)Math.Ceiling(mouseOffset));
            
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
        resizeBoxes = new Sides(
            new RightSide(new SelectionBox(new Vector2(windowSize.x, windowSize.y / 2), UIParams.minNearSelection, windowSize.y), graphics, window),
            new LeftSide(new SelectionBox(new Vector2(0, windowSize.y / 2), UIParams.minNearSelection, windowSize.y), graphics, window),
            new TopSide(new SelectionBox(new Vector2(windowSize.x / 2, 0), windowSize.x, UIParams.minNearSelection), graphics, window),
            new BottomSide(new SelectionBox(new Vector2(windowSize.x / 2, windowSize.y), windowSize.x, UIParams.minNearSelection), graphics, window)
        );
    }
    
    public override bool ShouldResize() => Program.instance.Fullscreen || base.ShouldResize();

    public override bool ResizeWindow()
    {
        if (!base.ResizeWindow())
            return false;
        var windowSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        Program.instance.onScreenSizeChange?.Invoke(windowSize);
        return true;
    }
}