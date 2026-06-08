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
    private static readonly Dictionary<Button, CustomButtonVisual> CustomButtons = [];
    private readonly RectangleRuntime buttonOutline;
    private readonly ButtonVisual visual;
    private SpriteRuntime? icon;

    private CustomButtonVisual(Button button)
    {
        this.visual = (ButtonVisual)button.Visual;

        this.buttonOutline = new RectangleRuntime
        {
            Width = this.visual.GetAbsoluteWidth(),
            Height = this.visual.GetAbsoluteHeight(),
            Color = Params.DefaultOutlineColor,
            LineWidth = Params.defaultOutLineWidth,
            Visible = this.visual.Visible,
            IgnoredByParentSize = true
        };

        this.visual.SizeChanged += (_, __) =>
        {
            this.buttonOutline.Width = this.visual.GetAbsoluteWidth();
            this.buttonOutline.Height = this.visual.GetAbsoluteHeight();
        };

        button.AddChild(this.buttonOutline);
        
        CustomButtons.Add(button, this);
        this.visual.ParentChanged += (_, __) =>
        {
            if(this.visual.Parent == null) // removed / disposed
            {
                CustomButtons.Remove(button);
            }
        };
    }

    private void SetIcon(SpriteRuntime sprite) => this.icon = sprite;

    private void EnabledState()
    {
        this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
        this.visual.Background.Color = Params.DefaultFillColor;
        this.buttonOutline.Color = Params.DefaultOutlineColor;
        this.visual.TextInstance.Color = Params.DefaultOutlineColor;
        if (this.icon != null) this.icon.Color = Params.DefaultOutlineColor;
    }

    private void PushState()
    {
        this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
        this.visual.Background.Color = Params.CanvasColor;
        this.buttonOutline.Color = Params.DefaultOutlineColor;
        this.visual.TextInstance.Color = Params.DefaultOutlineColor;
        if (this.icon != null) this.icon.Color = Params.DefaultOutlineColor;
    }

    private void HighlightedState()
    {
        this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
        this.visual.Background.Color = Params.DefaultOutlineColor;
        this.buttonOutline.Color = Params.DefaultFillColor;
        this.visual.TextInstance.Color = Params.DefaultFillColor;
        if (this.icon != null) this.icon.Color = Params.DefaultFillColor;
    }
    
    public static void Create(Button button)
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

    public static void AddIcon(Button button, Texture2D iconTex)
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

        if (!CustomButtons.TryGetValue(button, out CustomButtonVisual? buttonVisual)) return;
        buttonVisual.SetIcon(icon);
        ButtonVisual visual = (ButtonVisual)button.Visual;
        visual.HeightUnits = DimensionUnitType.ScreenPixel;
        visual.Background.ApplyState(visual.States.Enabled);
    }
}