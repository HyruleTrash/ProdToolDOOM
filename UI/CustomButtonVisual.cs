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

namespace DLLevelBuilder.UI;

public class CustomButtonVisual
{
    private static readonly Dictionary<ButtonVisual, CustomButtonVisual> CustomButtons = [];
    private readonly RectangleRuntime buttonOutline;
    private readonly ButtonVisual visual;
    private SpriteRuntime? icon;
    private bool shouldColorIcon;
    private CustomButtonTheme theme;
    
    public struct CustomButtonTheme(Color mainColor, Color secondaryColor, Color tertiaryColor)
    {
        public readonly Color mainColor = mainColor;
        public readonly Color secondaryColor = secondaryColor;
        public readonly Color tertiaryColor = tertiaryColor;
        
        public static CustomButtonTheme DefaultTheme = new CustomButtonTheme(Params.DefaultFillColor, Params.DefaultOutlineColor, Params.CanvasColor);
        public static CustomButtonTheme InvertedTheme = new CustomButtonTheme(Params.DefaultFillColor, Params.CanvasColor, Params.DefaultOutlineColor);
    }

    private CustomButtonVisual(ButtonVisual visual, CustomButtonTheme? theme)
    {
        this.visual = visual;
        this.theme = theme ?? CustomButtonTheme.DefaultTheme;
        
        this.buttonOutline = new RectangleRuntime
        {
            Width = this.visual.GetAbsoluteWidth(),
            Height = this.visual.GetAbsoluteHeight(),
            Color = this.theme.secondaryColor,
            LineWidth = Params.defaultOutLineWidth,
            Visible = this.visual.Visible,
            IgnoredByParentSize = true
        };

        this.visual.SizeChanged += (_, __) =>
        {
            this.buttonOutline.Width = this.visual.GetAbsoluteWidth();
            this.buttonOutline.Height = this.visual.GetAbsoluteHeight();
        };

        this.visual.AddChild(this.buttonOutline);
        
        CustomButtons.Add(this.visual, this);
        this.visual.ParentChanged += (_, __) =>
        {
            if(this.visual.Parent == null) // removed / disposed
                CustomButtons.Remove(this.visual);
        };
    }

    private void SetIcon(SpriteRuntime sprite, bool shouldColorIcon = true)
    {
        this.icon = sprite;
        this.shouldColorIcon = shouldColorIcon;
    }

    private void EnabledState()
    {
        this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
        this.visual.Background.Color = this.theme.mainColor;
        this.buttonOutline.Color = this.theme.secondaryColor;
        this.visual.TextInstance.Color = this.theme.secondaryColor;
        if (this.icon != null && this.shouldColorIcon) this.icon.Color = this.theme.secondaryColor;
    }
    
    private void AltEnabledState() => PushState();

    private void PushState()
    {
        this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
        this.visual.Background.Color = this.theme.tertiaryColor;
        this.buttonOutline.Color = this.theme.secondaryColor;
        this.visual.TextInstance.Color = this.theme.secondaryColor;
        if (this.icon != null && this.shouldColorIcon) this.icon.Color = this.theme.secondaryColor;
    }

    private void HighlightedState()
    {
        this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
        this.visual.Background.Color = this.theme.secondaryColor;
        this.buttonOutline.Color = this.theme.mainColor;
        this.visual.TextInstance.Color = this.theme.mainColor;
        if (this.icon != null && this.shouldColorIcon) this.icon.Color = this.theme.mainColor;
    }
    
    public static void Create(Button button, CustomButtonTheme? theme = null)
    {
        ButtonVisual visual = (ButtonVisual)button.Visual;
        Create(visual, theme);
    }
    
    public static void Create(ButtonVisual visual, CustomButtonTheme? theme = null) => Create(visual, new CustomButtonVisual(visual, theme));

    private static void Create(ButtonVisual visual, CustomButtonVisual customVisual)
    {
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

    public static void SetAltEnabledState(Button button, bool toSet)
    {
        ButtonVisual visual = (ButtonVisual)button.Visual;
        CustomButtons.TryGetValue(visual, out CustomButtonVisual? buttonVisual);
        if (buttonVisual == null) return;
        
        StateSave enabled = visual.States.Enabled;
        enabled.Clear();
        enabled.Apply = toSet ? buttonVisual.AltEnabledState : buttonVisual.EnabledState;
        visual.ApplyState(enabled);
    }
    
    public static void AddIcon(Button button, Texture2D iconTex, bool shouldColorIcon = true)
    {
        ButtonVisual visual = (ButtonVisual)button.Visual;
        SpriteRuntime icon = new()
        {
            Texture = iconTex,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = iconTex.Width,
            TextureHeight = iconTex.Height,
            IgnoredByParentSize = true
        };
        icon.Anchor(Anchor.Center);
        visual.AddChild(icon);

        if (!CustomButtons.TryGetValue(visual, out CustomButtonVisual? buttonVisual)) return;
        buttonVisual.SetIcon(icon, shouldColorIcon);
        visual.HeightUnits = DimensionUnitType.ScreenPixel;
        visual.Background.ApplyState(visual.States.Enabled);
    }
}