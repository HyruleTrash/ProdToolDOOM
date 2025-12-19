using Gum.DataTypes.Variables;
using Gum.Forms.DefaultVisuals;
using Color = Microsoft.Xna.Framework.Color;

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
    
    public const float minNearSelection = 10;

    public static void SetDefaultButton(ButtonVisual visual)
    {
        var enabled = visual.States.Enabled;
        enabled.Clear();
        enabled.Apply = () =>
        {
            visual.Background.Color = Color.Gray;
        };
        
        var pushed = visual.States.Pushed;
        pushed.Clear();
        pushed.Apply = () =>
        {
            visual.Background.Color = Color.DarkSlateGray;
        };
        
        var highlighted = visual.States.Highlighted;
        highlighted.Clear();
        highlighted.Apply = () =>
        {
            visual.Background.Color = Color.DimGray;
        };
        
        visual.ApplyState(enabled);
    }
}