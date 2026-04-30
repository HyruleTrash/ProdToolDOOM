using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using TextBox = Gum.Forms.Controls.TextBox;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures;

public class EntityCreationPopup : Popup<EntityCreationPopup>
{
    private readonly StackPanel panel;
    private readonly TextBox textBox;
    private readonly ColoredRectangleRuntime popupBG;
    private readonly RectangleRuntime popupBGBorder;
    private Button confirmButton;
    private string? lastSavedText;

    public EntityCreationPopup()
    {
        this.panel = new StackPanel { Spacing = 5, };
        this.popupBG = new ColoredRectangleRuntime { Color = UIParams.defaultFillColor };
        this.popupBGBorder = new RectangleRuntime { Color = UIParams.defaultOutlineColor };

        this.textBox = new TextBox()
        {
            Width = 200,
            Height = 40,
            Placeholder = "Enter name...",
        };
        
        this.confirmButton = new Button
        {
            Text = "Confirm",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.confirmButton);
        this.confirmButton.Click += (_, _) => ConfirmCreation();
        
        this.container.AddChild(this.popupBG);
        this.container.AddChild(this.popupBGBorder);
        this.container.AddChild(this.panel.Visual);
        this.panel.AddChild(this.textBox);
        this.panel.AddChild(this.confirmButton);

        this.textBox.TextChanged += (_, _) => this.lastSavedText = this.textBox.Text;
        
        UpdatePositionsAndSizes();
    }

    private void ConfirmCreation()
    {
        if (this.lastSavedText == null) return;
        try
        {
            Program.instance.cmdHistory.ApplyCmd(new AddEntityDataCmd(Project.instance, this.lastSavedText));
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            this.textBox.Text = "";
            this.lastSavedText = null;
            ToggleVisibility();
        }
    }

    private void UpdatePanelSize()
    {
        float biggestWidth = this.panel.Children.Select(child => child.Width).Prepend(0).Max();
        this.panel.Width = biggestWidth;
        float totalHeight = this.panel.Children.Sum(child => child.ActualHeight);
        this.panel.Height = totalHeight;
    }

    protected override void UpdatePositionsAndSizes()
    {
        UpdatePanelSize();
        
        base.UpdatePositionsAndSizes();
            
        this.popupBG.Width = this.panel.Width + UIParams.popupPadding;
        this.popupBG.Height = this.panel.Height + UIParams.popupPadding;
        this.popupBG.X = this.popUpContainerRef.Width / 2 - this.popupBG.Width / 2;
        this.popupBG.Y = this.popUpContainerRef.Height / 2 - this.popupBG.Height / 2;
        
        this.popupBGBorder.Width = this.panel.Width + UIParams.popupPadding + UIParams.defaultOutLineWidth;
        this.popupBGBorder.Height = this.panel.Height + UIParams.popupPadding + UIParams.defaultOutLineWidth;
        this.popupBGBorder.X = this.popUpContainerRef.Width / 2 - this.popupBGBorder.Width / 2;
        this.popupBGBorder.Y = this.popUpContainerRef.Height / 2 - this.popupBGBorder.Height / 2;
            
        this.panel.X = this.popUpContainerRef.Width / 2 - this.panel.Width / 2;
        this.panel.Y = this.popUpContainerRef.Height / 2 - this.panel.Height / 2;
        
        this.confirmButton.Width = this.panel.Width;
    }
}