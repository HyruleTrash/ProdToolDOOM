using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Forms.DefaultVisuals;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using Color = Microsoft.Xna.Framework.Color;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM;

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

    // button specific
    private static Dictionary<Button, CustomButtonVisual> custombuttons = [];
    
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

    public static void AddIconToButton(Button button, Texture2D iconTex)
    {
        SpriteRuntime icon = new SpriteRuntime
        {
            Texture = iconTex,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = iconTex.Width,
            TextureHeight = iconTex.Height,
            IgnoredByParentSize = true
        };
        icon.Anchor(Anchor.Center);
        button.AddChild(icon);

        if (!custombuttons.TryGetValue(button, out CustomButtonVisual? buttonVisual)) return;
        buttonVisual.SetIcon(icon);
        ButtonVisual visual = (ButtonVisual)button.Visual;
        visual.HeightUnits = DimensionUnitType.ScreenPixel;
        visual.Background.ApplyState(visual.States.Enabled);
    }
    
    private class CustomButtonVisual
    {
        private Button button;
        private RectangleRuntime buttonOutline;
        private ButtonVisual visual;
        private SpriteRuntime? icon;

        public CustomButtonVisual(Button button)
        {
            this.button = button;
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
            
            custombuttons.Add(button, this);
            this.visual.ParentChanged += (_, __) =>
            {
                if(this.visual.Parent == null) // removed / disposed
                {
                    custombuttons.Remove(button);
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
}