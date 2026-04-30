using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;

namespace ProdToolDOOM.ProjectFeatures;

public class EntityManagerPopup : Popup<EntityManagerPopup>
{
    private readonly StackPanel panel;
    private readonly ColoredRectangleRuntime popupBG;
    private readonly RectangleRuntime popupBGBorder;

    public EntityManagerPopup()
    {
        this.panel = new StackPanel { Spacing = 5, };
        this.popupBG = new ColoredRectangleRuntime { Color = UIParams.defaultFillColor };
        this.popupBGBorder = new RectangleRuntime { Color = UIParams.defaultOutlineColor };
        
        this.container.AddChild(this.popupBG);
        this.container.AddChild(this.popupBGBorder);
        this.container.AddChild(this.panel.Visual);
        
        UpdatePositionsAndSizes();
    }
    
    protected override void UpdatePositionsAndSizes()
    {
        base.UpdatePositionsAndSizes();

        const float popupWidth = 400f;
        const float popupHeight = 300f;
        const float margin = 16f;

        float containerWidth = this.popUpContainerRef.Width;
        // float containerHeight = this.popUpContainerRef.Height;

        // Top-right anchor
        float popupX = containerWidth - popupWidth - margin;
        float popupY = margin + UIParams.minBoxSize;

        // Background
        this.popupBG.Width = popupWidth;
        this.popupBG.Height = popupHeight;
        this.popupBG.X = popupX;
        this.popupBG.Y = popupY;

        // Background border
        this.popupBGBorder.Width = popupWidth + UIParams.defaultOutLineWidth;
        this.popupBGBorder.Height = popupHeight + UIParams.defaultOutLineWidth;
        this.popupBGBorder.X = popupX - UIParams.defaultOutLineWidth / 2;
        this.popupBGBorder.Y = popupY - UIParams.defaultOutLineWidth / 2;

        // Panel
        this.panel.Width = popupWidth - UIParams.popupPadding;
        this.panel.Height = popupHeight - UIParams.popupPadding;
        this.panel.X = popupX + UIParams.popupPadding / 2;
        this.panel.Y = popupY + UIParams.popupPadding / 2;
    }
}