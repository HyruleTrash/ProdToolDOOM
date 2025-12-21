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

    // button specific
    private static Dictionary<Button, CustomButtonVisual> custombuttons = [];
    
    public static void SetDefaultButton(Button button)
    {
        CustomButtonVisual customVisual = new(button);
        ButtonVisual visual = (ButtonVisual)button.Visual;
        
        var enabled = visual.States.Enabled;
        enabled.Clear();
        enabled.Apply = customVisual.EnabledState;
        
        var pushed = visual.States.Pushed;
        pushed.Clear();
        pushed.Apply = customVisual.PushState;
        
        var highlighted = visual.States.Highlighted;
        highlighted.Clear();
        highlighted.Apply = customVisual.HighlightedState;
        
        visual.ApplyState(enabled);
    }

    public static void AddIconToButton(Button button, Texture2D iconTex)
    {
        var icon = new SpriteRuntime
        {
            Texture = iconTex,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = iconTex.Width,
            TextureHeight = iconTex.Height,
            IgnoredByParentSize = true
        };
        icon.Anchor(Anchor.Center);
        button.AddChild(icon);

        if (!custombuttons.TryGetValue(button, out var buttonVisual)) return;
        buttonVisual.SetIcon(icon);
        var visual = (ButtonVisual)button.Visual;
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
            visual = (ButtonVisual)button.Visual;
            
            buttonOutline = new RectangleRuntime
            {
                Width = visual.GetAbsoluteWidth(),
                Height = visual.GetAbsoluteHeight(),
                Color = defaultOutlineColor,
                LineWidth = defaultOutLineWidth,
                Visible = visual.Visible,
                IgnoredByParentSize = true
            };
            
            visual.SizeChanged += (_, __) =>
            {
                buttonOutline.Width = visual.GetAbsoluteWidth();
                buttonOutline.Height = visual.GetAbsoluteHeight();
            };

            button.AddChild(buttonOutline);
            
            custombuttons.Add(button, this);
            visual.ParentChanged += (_, __) =>
            {
                if(visual.Parent == null) // removed / disposed
                {
                    custombuttons.Remove(button);
                }
            };
        }
        
        public void SetIcon(SpriteRuntime sprite) => icon = sprite;
        
        public void EnabledState()
        {
            visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid); 
            visual.Background.Color = defaultFillColor;
            buttonOutline.Color = defaultOutlineColor;
            visual.TextInstance.Color = defaultOutlineColor;
            if (icon != null) icon.Color = defaultOutlineColor;
        }

        public void PushState()
        {
            visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid); 
            visual.Background.Color = canvasColor;
            buttonOutline.Color = defaultOutlineColor;
            visual.TextInstance.Color = defaultOutlineColor;
            if (icon != null) icon.Color = defaultOutlineColor;
        }

        public void HighlightedState()
        {
            visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid); 
            visual.Background.Color = defaultOutlineColor;
            buttonOutline.Color = defaultFillColor;
            visual.TextInstance.Color = defaultFillColor;
            if (icon != null) icon.Color = defaultFillColor;
        }
    }
}