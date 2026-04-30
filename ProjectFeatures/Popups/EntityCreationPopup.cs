using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using Color = Microsoft.Xna.Framework.Color;
using TextBox = Gum.Forms.Controls.TextBox;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures.Popups;

public class EntityCreationPopup
{
    public static EntityCreationPopup Instance
    {
        get
        {
            instance ??= new EntityCreationPopup();
            return instance;
        }
        private set => instance = value;
    }
    private static EntityCreationPopup instance = null!;
    
    private readonly ContainerRuntime container;
    private readonly StackPanel panel;
    private readonly TextBox textBox;
    private readonly ColoredRectangleRuntime transparentOvelay;
    private readonly ColoredRectangleRuntime popupBG;
    private readonly RectangleRuntime popupBGBorder;
    private Button confirmButton;
    private string? lastSavedText;

    private const float popupBGPadding = 50;
    
    public EntityCreationPopup()
    {
        ContainerRuntime canvas = Project.instance.popUpContainer;
        this.container = new ContainerRuntime { Visible = false };

        Color transparentBlack = Color.Black;
        transparentBlack.A = 50;
        this.transparentOvelay = new ColoredRectangleRuntime { Color = transparentBlack };
        
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
        
        canvas.AddChild(this.container);
        this.container.AddChild(this.transparentOvelay);
        this.container.AddChild(this.popupBG);
        this.container.AddChild(this.popupBGBorder);
        this.container.AddChild(this.panel.Visual);
        this.panel.AddChild(this.textBox);
        this.panel.AddChild(this.confirmButton);

        UpdatePositionsAndSizes(canvas);
        canvas.SizeChanged += (_, _) => UpdatePositionsAndSizes(Project.instance.popUpContainer);
        this.textBox.TextChanged += (_, _) => this.lastSavedText = this.textBox.Text;
        this.container.Click += (_, _) => ToggleVisibility();
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

    private void UpdatePositionsAndSizes(ContainerRuntime canvas)
    {
        UpdatePanelSize();
        this.container.Width = canvas.Width;
        this.container.Height = canvas.Height;
        this.transparentOvelay.Width = canvas.Width;
        this.transparentOvelay.Height = canvas.Height;
            
        this.popupBG.Width = this.panel.Width + popupBGPadding;
        this.popupBG.Height = this.panel.Height + popupBGPadding;
        this.popupBG.X = canvas.Width / 2 - this.popupBG.Width / 2;
        this.popupBG.Y = canvas.Height / 2 - this.popupBG.Height / 2;
        
        this.popupBGBorder.Width = this.panel.Width + popupBGPadding + UIParams.defaultOutLineWidth;
        this.popupBGBorder.Height = this.panel.Height + popupBGPadding + UIParams.defaultOutLineWidth;
        this.popupBGBorder.X = canvas.Width / 2 - this.popupBGBorder.Width / 2;
        this.popupBGBorder.Y = canvas.Height / 2 - this.popupBGBorder.Height / 2;
            
        this.panel.X = canvas.Width / 2 - this.panel.Width / 2;
        this.panel.Y = canvas.Height / 2 - this.panel.Height / 2;
        
        this.confirmButton.Width = this.panel.Width;
    }

    public static void ToggleVisibility() => Instance.container.Visible = !Instance.container.Visible;
}