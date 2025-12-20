using Gum.DataTypes.Variables;
using Gum.Forms.DefaultVisuals;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using Color = Microsoft.Xna.Framework.Color;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM;

public static class UIParams
{
    public const int minWindowWidth = 500;
    public const int minWindowHeight = 100;
    public const int minResizePerFrame = 200;
    
    public const float borderPadding = 10;
    public const float borderRadius = 5;

    public const float minBoxSize = 32;
    public const float minButtonHeight = (minBoxSize / 2 - defaultFontSize / 2) - 1;
    public const float defaultFontSize = 18;
    public const float defaultLineWidth = 2;
    
    public const float minNearSelection = 10;
    
    public static readonly Color defaultFillColor = new (206, 209, 214);
    public static readonly Color defaultOutlineColor = new (175, 153, 222);
    public static readonly Color canvasColor = new (36, 28, 47);

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
}

public class CustomButtonVisual
{
    private RectangleRuntime buttonOutline;
    private ButtonVisual visual;

    public CustomButtonVisual(Button button)
    {
        visual = (ButtonVisual)button.Visual;
        
        buttonOutline = new RectangleRuntime
        {
            Width = visual.GetAbsoluteWidth(),
            Height = visual.GetAbsoluteHeight(),
            Color = UIParams.defaultOutlineColor,
            LineWidth = UIParams.defaultLineWidth,
            Visible = visual.Visible,
            IgnoredByParentSize = true
        };
        
        visual.SizeChanged += (_, __) =>
        {
            buttonOutline.Width = visual.GetAbsoluteWidth();
            buttonOutline.Height = visual.GetAbsoluteHeight();
        };

        button.AddChild(buttonOutline);
    }
    
    public void EnabledState()
    {
        visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid); 
        visual.Background.Color = UIParams.defaultFillColor;
        buttonOutline.Color = UIParams.defaultOutlineColor;
    }

    public void PushState()
    {
        visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid); 
        visual.Background.Color = UIParams.canvasColor;
        buttonOutline.Color = UIParams.defaultOutlineColor;
    }

    public void HighlightedState()
    {
        visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid); 
        visual.Background.Color = UIParams.defaultOutlineColor;
        buttonOutline.Color = UIParams.defaultFillColor;
    }
}