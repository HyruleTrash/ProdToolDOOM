using Gum.Converters;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder;

public static class UIParams
{
    // window specific
    public const int minWindowWidth = 500;
    public const int minWindowHeight = 100;
    public const int minResizePerFrame = 200;
    
    // box specific
    public const float borderPadding = 10;
    public const float borderRadius = 5;

    public const float minBoxSize = 32;
    public const float defaultOutLineWidth = 2;
    
    // dealing with text in buttons
    public const float minButtonHeight = (minBoxSize / 2 - defaultFontSize / 2) - 1;
    public const float defaultFontSize = 18;
    
    // selection box
    public const float minNearSelection = 10;
    
    // ui colors
    public static readonly Color defaultFillColor = new (206, 209, 214);
    public static readonly Color defaultOutlineColor = new (175, 153, 222);
    public static readonly Color canvasColor = new (36, 28, 47);
    public static readonly Color selectionColor = new (96, 101, 234);

    // tracking lists
    private static readonly Dictionary<Button, CustomButtonVisual> customButtons = [];
    private static readonly Dictionary<MenuItem, CustomMenuItemVisual> customMenuItem = [];
    
    // popups
    public const float popupPadding = 50f;
    
    public static void SetDefaultButton(Button button)
    {
        CustomButtonVisual customVisual = new(button);
        ButtonVisual visual = (ButtonVisual)button.Visual;
        
        StateSave enabled = visual.States.Enabled;
        enabled.Clear();
        enabled.Apply = customVisual.EnabledState;
        
        StateSave pushed = visual.States.Pushed;
        pushed.Clear();
        pushed.Apply = customVisual.PushState;
        
        StateSave highlighted = visual.States.Highlighted;
        highlighted.Clear();
        highlighted.Apply = customVisual.HighlightedState;
        
        visual.ApplyState(enabled);
    }

    public static void SetDefaultMenuItem(MenuItem menuItem)
    {
        CustomMenuItemVisual customVisual = new(menuItem);
        MenuItemVisual visual = (MenuItemVisual)menuItem.Visual;
        
        StateSave enabled = visual.States.Enabled;
        enabled.Clear();
        enabled.Apply = customVisual.EnabledState;
        
        StateSave selected = visual.States.Selected;
        selected.Clear();
        selected.Apply = customVisual.SelectedState;
        
        StateSave highlighted = visual.States.Highlighted;
        highlighted.Clear();
        highlighted.Apply = customVisual.HighlightedState;
        
        visual.ApplyState(enabled);
    }

    public static void AddIconToButton(Button button, Texture2D iconTex)
    {
        SpriteRuntime icon = new()
        {
            Texture = iconTex,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = iconTex.Width,
            TextureHeight = iconTex.Height,
            IgnoredByParentSize = true
        };
        icon.Anchor(Anchor.Center);
        button.AddChild(icon);

        if (!customButtons.TryGetValue(button, out CustomButtonVisual? buttonVisual)) return;
        buttonVisual.SetIcon(icon);
        ButtonVisual visual = (ButtonVisual)button.Visual;
        visual.HeightUnits = DimensionUnitType.ScreenPixel;
        visual.Background.ApplyState(visual.States.Enabled);
    }
    
    public static void AddIconToMenuItem(MenuItem menuItem, Texture2D iconTex)
    {
        menuItem.Header = "";
        SpriteRuntime icon = new()
        {
            Texture = iconTex,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = iconTex.Width,
            TextureHeight = iconTex.Height,
            IgnoredByParentSize = true
        };
        icon.Anchor(Anchor.Center);
        menuItem.AddChild(icon);

        if (!customMenuItem.TryGetValue(menuItem, out CustomMenuItemVisual? menuItemVisual)) return;
        menuItemVisual.SetIcon(icon);
        MenuItemVisual visual = (MenuItemVisual)menuItem.Visual;
        visual.HeightUnits = DimensionUnitType.ScreenPixel;
        visual.Background.ApplyState(visual.States.Enabled);
    }
    
    private class CustomButtonVisual
    {
        private readonly RectangleRuntime buttonOutline;
        private readonly ButtonVisual visual;
        private SpriteRuntime? icon;

        public CustomButtonVisual(Button button)
        {
            this.visual = (ButtonVisual)button.Visual;

            this.buttonOutline = new RectangleRuntime
            {
                Width = this.visual.GetAbsoluteWidth(),
                Height = this.visual.GetAbsoluteHeight(),
                Color = defaultOutlineColor,
                LineWidth = defaultOutLineWidth,
                Visible = this.visual.Visible,
                IgnoredByParentSize = true
            };

            this.visual.SizeChanged += (_, __) =>
            {
                this.buttonOutline.Width = this.visual.GetAbsoluteWidth();
                this.buttonOutline.Height = this.visual.GetAbsoluteHeight();
            };

            button.AddChild(this.buttonOutline);
            
            customButtons.Add(button, this);
            this.visual.ParentChanged += (_, __) =>
            {
                if(this.visual.Parent == null) // removed / disposed
                {
                    customButtons.Remove(button);
                }
            };
        }
        
        public void SetIcon(SpriteRuntime sprite) => this.icon = sprite;
        
        public void EnabledState()
        {
            this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
            this.visual.Background.Color = defaultFillColor;
            this.buttonOutline.Color = defaultOutlineColor;
            this.visual.TextInstance.Color = defaultOutlineColor;
            if (this.icon != null) this.icon.Color = defaultOutlineColor;
        }

        public void PushState()
        {
            this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
            this.visual.Background.Color = canvasColor;
            this.buttonOutline.Color = defaultOutlineColor;
            this.visual.TextInstance.Color = defaultOutlineColor;
            if (this.icon != null) this.icon.Color = defaultOutlineColor;
        }

        public void HighlightedState()
        {
            this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
            this.visual.Background.Color = defaultOutlineColor;
            this.buttonOutline.Color = defaultFillColor;
            this.visual.TextInstance.Color = defaultFillColor;
            if (this.icon != null) this.icon.Color = defaultFillColor;
        }
    }
    
    private class CustomMenuItemVisual
    {
        private readonly MenuItem menuItem;
        private readonly RectangleRuntime outline;
        private readonly MenuItemVisual visual;
        private SpriteRuntime? icon;

        public CustomMenuItemVisual(MenuItem menuItem)
        {
            this.menuItem = menuItem;
            this.visual = (MenuItemVisual)menuItem.Visual;
            
            this.outline = new RectangleRuntime
            {
                X = 0,
                Y = 0,
                XUnits = GeneralUnitType.PixelsFromMiddle,
                YUnits = GeneralUnitType.PixelsFromMiddle,
                XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Center,
                YOrigin = VerticalAlignment.Center,
                Width = 0,
                Height = 0,
                WidthUnits = DimensionUnitType.RelativeToParent,
                HeightUnits = DimensionUnitType.RelativeToParent, 
                Color = defaultOutlineColor,
                LineWidth = defaultOutLineWidth,
                Visible = this.visual.Visible,
                IgnoredByParentSize = true
            };

            menuItem.AddChild(this.outline);
            
            customMenuItem.Add(menuItem, this);
            this.visual.ParentChanged += (_, __) =>
            {
                if (this.visual.Parent == null) // removed / disposed
                    customMenuItem.Remove(menuItem);
            };

            CheckMinSizes();
        }

        private void CheckMinSizes()
        {
            CheckMinHeight();
            CheckMinWidth();
        }

        private void CheckMinHeight()
        {
            this.visual.HeightUnits = DimensionUnitType.RelativeToChildren;
            this.visual.UpdateLayout();

            if (!(this.visual.ContainerInstance.GetAbsoluteHeight() < minBoxSize)) return;
            this.visual.HeightUnits = DimensionUnitType.Absolute;
            this.menuItem.Height = minBoxSize;
            this.visual.ContainerInstance.YOrigin = VerticalAlignment.Center;
            this.visual.ContainerInstance.YUnits = GeneralUnitType.PixelsFromMiddle;
            this.visual.ContainerInstance.Y = -2;
        }

        private void CheckMinWidth()
        {
            this.visual.WidthUnits = DimensionUnitType.RelativeToChildren;
            this.visual.UpdateLayout();

            if (!(this.visual.ContainerInstance.GetAbsoluteWidth() < minBoxSize)) return;
            this.visual.WidthUnits = DimensionUnitType.Absolute;
            this.menuItem.Width = minBoxSize;
        }
        
        public void SetIcon(SpriteRuntime sprite) => this.icon = sprite;

        private void BaseState()
        {
            this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
            this.visual.Background.Visible = true;
            CheckMinSizes();
        }
        
        public void EnabledState()
        {
            BaseState();
            this.visual.Background.Color = defaultFillColor;
            this.outline.Color = defaultOutlineColor;
            this.outline.Visible = true;
            this.visual.TextInstance.Color = defaultOutlineColor;
            if (this.icon != null) this.icon.Color = defaultOutlineColor;
        }

        public void HighlightedState()
        {
            BaseState();
            this.visual.Background.Color = defaultOutlineColor;
            this.outline.Color = defaultFillColor;
            this.visual.TextInstance.Color = defaultFillColor;
            if (this.icon != null) this.icon.Color = defaultFillColor;
        }

        public void SelectedState()
        {
            BaseState();
            this.visual.Background.Color = canvasColor;
            this.outline.Color = defaultOutlineColor;
            this.visual.TextInstance.Color = defaultOutlineColor;
        }
    }
}