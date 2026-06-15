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

public static class Params
{
    public const float clickCaptureMs = 300;

    // window specific
    public const int minWindowWidth = 500;
    public const int minWindowHeight = 100;
    public const int minResizePerFrame = 200;
    
    // box specific
    public const float borderPadding = 10;
    public const float borderMargin = 20;

    public const float minBoxSize = 32;
    public const float defaultOutLineWidth = 2;
    
    // dealing with text in buttons
    public const float minButtonHeight = (minBoxSize / 2 - defaultFontSize / 2) - 1;
    public const float defaultFontSize = 18;
    
    // selection box
    public const float minNearSelection = 10;
    
    // ui colors
    public static readonly Color DefaultFillColor = new (206, 209, 214);
    public static readonly Color DefaultOutlineColor = new (175, 153, 222);
    public static readonly Color CanvasColor = new (36, 28, 47);
    public static readonly Color grayish = new (178, 182, 190);
    public static readonly Color SelectionColor = new (96, 101, 234);
    
    // popups
    public const float popupPadding = 25f;
}