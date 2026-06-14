using DLLevelBuilder.UI;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using Button = Gum.Forms.Controls.Button;
using HorizontalAlignment = RenderingLibrary.Graphics.HorizontalAlignment;
using Orientation = Gum.Forms.Controls.Orientation;

namespace DLLevelBuilder.ProjectFeatures;

public class ConfirmationPopup : Popup<ConfirmationPopup>
{
    private static string text = "";
    private static string Text
    {
        get => text;
        set
        {
            text = value;
            OnTextChanged?.Invoke(value);
        }
    }
    private static Action<string>? OnTextChanged;

    private static Action? OnConfirmation;
    private readonly ContainerRuntime panel;
    private readonly ColoredRectangleRuntime popupBG;
    private readonly RectangleRuntime popupBGBorder;
    private readonly StackPanel contentStack;
    private readonly TextRuntime textUI;
    private readonly StackPanel buttonStack;
    private readonly Button confirmButton;
    private readonly Button declineButton;

    public ConfirmationPopup()
    {
        this.panel = new ContainerRuntime
        {
            XUnits = GeneralUnitType.PixelsFromMiddle,
            YUnits = GeneralUnitType.PixelsFromMiddle,
            XOrigin = HorizontalAlignment.Center,
            YOrigin = VerticalAlignment.Center,
            WidthUnits = DimensionUnitType.RelativeToChildren,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 0,
            Height = 0
        };
            
        this.contentStack = new StackPanel
        {
            Spacing = 5,
            Visual = {
                WidthUnits = DimensionUnitType.RelativeToChildren, 
                HeightUnits = DimensionUnitType.RelativeToChildren 
            },
            Width = 0,
            Height = 0
        };

        this.popupBG = new ColoredRectangleRuntime
        {
            Color = Params.DefaultFillColor,
            IgnoredByParentSize = true,
            WidthUnits = DimensionUnitType.RelativeToParent,
            Width = Params.popupPadding,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Height = Params.popupPadding,
            XUnits = GeneralUnitType.PixelsFromMiddle,
            YUnits = GeneralUnitType.PixelsFromMiddle,
            XOrigin = HorizontalAlignment.Center,
            YOrigin = VerticalAlignment.Center
        };
        this.popupBGBorder = new RectangleRuntime
        {
            Color = Params.DefaultOutlineColor,
            IgnoredByParentSize = true,
            WidthUnits = DimensionUnitType.RelativeToParent,
            Width = Params.defaultOutLineWidth,
            HeightUnits = DimensionUnitType.RelativeToParent,
            Height = Params.defaultOutLineWidth,
            XUnits = GeneralUnitType.PixelsFromMiddle,
            YUnits = GeneralUnitType.PixelsFromMiddle,
            XOrigin = HorizontalAlignment.Center,
            YOrigin = VerticalAlignment.Center
        };

        this.textUI = new TextRuntime
        {
            Text = Text,
            Color = Params.DefaultOutlineColor,
            WidthUnits = DimensionUnitType.RelativeToParent,
            Wrap = true,
            Width = 0
        };
        OnTextChanged += s => this.textUI.Text = s;
        
        this.buttonStack = new StackPanel
        {
            Spacing = 5, Orientation = Orientation.Horizontal,
            Visual = { WidthUnits = DimensionUnitType.RelativeToChildren },
            Width = 0
        };
        
        this.confirmButton = new Button
        {
            Text = "Yes",
            Height = Params.minButtonHeight
        };
        CustomButtonVisual.Create(this.confirmButton);
        this.confirmButton.Click += (_, _) => ConfirmCreation();
        
        this.declineButton = new Button
        {
            Text = "No",
            Height = Params.minButtonHeight
        };
        CustomButtonVisual.Create(this.declineButton);
        this.declineButton.Click += (_, _) => ToggleVisibility();
        
        this.container.AddChild(this.panel);
        this.panel.AddChild(this.popupBG);
        this.popupBG.AddChild(this.popupBGBorder);
        this.panel.AddChild(this.contentStack.Visual);
        this.contentStack.AddChild(this.textUI);
        this.contentStack.AddChild(this.buttonStack);
        this.buttonStack.AddChild(this.confirmButton);
        this.buttonStack.AddChild(this.declineButton);
        
        UpdatePositionsAndSizes();
    }

    private void ConfirmCreation()
    {
        if (OnConfirmation == null) return;
        OnConfirmation.Invoke();
        ToggleVisibility();
        UpdatePositionsAndSizes();
    }

    public static void SetAndToggleVisibility(string text, Action applyRemoveLevelCmd)
    {
        Text = text;
        OnConfirmation = applyRemoveLevelCmd;
        ToggleVisibility();
    }
}