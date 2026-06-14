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

public class EntityDataVisual
{
    public int? id;
    public EntityData? entityData;
    private readonly ContainerRuntime panel;
    private readonly TextRuntime nameText;
    private readonly Button removeButton;
    private readonly ColoredRectangleRuntime background;
    private readonly ContainerRuntime innerPanel;

    public EntityDataVisual(Texture2D closeIcon, StackPanel parent, int? id = null, EntityData? entityData = null)
    {
        this.id = id;
        this.entityData = entityData;
        
        Debug.Log($"Instantiating EntityDataVisual {this.id}");
        
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
            Text = entityData?.Name ?? "Unnamed",
            X = 8,
            Color = Color.Black
        };
        this.nameText.Anchor(Anchor.Left);

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

        this.panel.AddChild(this.background);
        this.panel.AddChild(this.innerPanel);
        this.innerPanel.AddChild(this.nameText);
        this.innerPanel.AddChild(this.removeButton.Visual);
        
        parent.AddChild(this.panel);

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        this.innerPanel.UpdateLayout();
        this.panel.UpdateLayout();
        if (this.id == null || this.entityData == null)
            return;
        this.panel.Visible = true;
    }

    public void RemoveAndHide()
    {
        this.panel.Visible = false;
        if (this.id != null)
            Program.instance.cmdHistory.ApplyCmd(new RemoveEntityDataCmd(Project.Instance, this.id, OnUndoRedo));
        this.id = null;
        this.entityData = null;
    }

    private void OnUndoRedo(int? newId, EntityData? newData)
    {
        if (newId == null)
        {
            this.id = null;
            RemoveAndHide();
            return;
        }

        this.id = newId;
        this.entityData = newData;
        UpdateVisuals();
    }
}