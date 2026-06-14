using DLLevelBuilder.UI;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using Button = Gum.Forms.Controls.Button;
using Color = Microsoft.Xna.Framework.Color;
using HorizontalAlignment = RenderingLibrary.Graphics.HorizontalAlignment;

namespace DLLevelBuilder.ProjectFeatures;

public class LevelVisual
{
    public int? id;
    public Level? levelData;
    private readonly ContainerRuntime panel;
    private readonly TextRuntime nameText;
    private readonly Button removeButton;
    private readonly Button openButton;
    private readonly ColoredRectangleRuntime background;
    private readonly ContainerRuntime innerPanel;
    private readonly Project projectRef;

    public LevelVisual(Texture2D closeIcon, StackPanel parent, int? id = null, Level? levelData = null)
    {
        this.id = id;
        this.levelData = levelData;
        
        this.projectRef  = Project.Instance;
        
        // instantiate visuals
        this.panel = new ContainerRuntime
        {
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 98,
            Height = 0
        };
        
        this.innerPanel = new ContainerRuntime
        {
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.RelativeToChildren,
            Width = 98,
            Height = 0,
            XUnits = GeneralUnitType.PixelsFromMiddle,
            YUnits = GeneralUnitType.PixelsFromMiddle,
            XOrigin = HorizontalAlignment.Center,
            YOrigin = VerticalAlignment.Center
        };
        this.nameText = new TextRuntime
        {
            Text = $"Level - {this.id}",
            Color = Color.Black
        };
        this.nameText.Anchor(Anchor.Left);
        this.nameText.X = 8;

        this.background = new ColoredRectangleRuntime()
        {
            Color = Params.grayish,
            WidthUnits = DimensionUnitType.PercentageOfParent,
            HeightUnits = DimensionUnitType.PercentageOfParent,
            Width = 102,
            Height = 120,
            XUnits = GeneralUnitType.PixelsFromMiddle,
            YUnits = GeneralUnitType.PixelsFromMiddle,
            XOrigin = HorizontalAlignment.Center,
            YOrigin = VerticalAlignment.Center
        };

        this.removeButton = new Button
        {
            Text = "",
            Width = Params.minBoxSize,
            Height = Params.minBoxSize
        };
        CustomButtonVisual.Create(this.removeButton);
        CustomButtonVisual.AddIcon(this.removeButton, closeIcon);
        this.removeButton.Anchor(Anchor.Right);
        this.removeButton.X = 0;
        this.removeButton.Click += (_, _) => RemoveAndHide();

        this.openButton = new Button { Text = "Open" };
        CustomButtonVisual.Create(this.openButton);
        this.openButton.Anchor(Anchor.Right);
        this.openButton.X = -this.removeButton.Width - Params.borderPadding;
        this.openButton.Click += (_, _) => SetToCurrentLevel();

        this.panel.AddChild(this.background);
        this.panel.AddChild(this.innerPanel);
        this.innerPanel.AddChild(this.nameText);
        this.innerPanel.AddChild(this.removeButton.Visual);
        this.innerPanel.AddChild(this.openButton.Visual);
        
        parent.AddChild(this.panel);

        UpdateVisuals();
    }

    private void SetToCurrentLevel()
    {
        if (this.id == null || this.projectRef.CurrentLevel == this.id.Value) return;
        Program.instance.cmdHistory.ApplyCmd(new SwitchLevelCmd(this.projectRef, null, this.id));
    }

    public void UpdateVisuals()
    {
        this.innerPanel.UpdateLayout();
        this.panel.UpdateLayout();
        if (this.id == null || this.levelData == null)
            return;
        this.panel.Visible = true;
    }

    public void RemoveAndHide()
    {
        if (this.id != null && this.levelData != null) 
            ConfirmationPopup.SetAndToggleVisibility($"Are you sure you wish to remove level {this.id.Value}?", () =>
            {
                this.panel.Visible = false;
                ApplyRemoveLevelCmd(this.id.Value, this.levelData);
                this.id = null;
                this.levelData = null;
            });
    }

    private void OnUndoRedo(int? newId, Level? newData)
    {
        if (newId == null)
        {
            this.id = null;
            RemoveAndHide();
            return;
        }

        this.id = newId;
        this.levelData = newData;
        UpdateVisuals();
    }
    
    private void ApplyRemoveLevelCmd(int id, Level level) => Program.instance.cmdHistory.ApplyCmd(new RemoveLevelCmd(this.projectRef, id, level, OnUndoRedo));
}