using System.Collections.Generic;
using System.Linq;
using DLLevelBuilder.UI;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using Button = Gum.Forms.Controls.Button;
using Color = Microsoft.Xna.Framework.Color;
using HorizontalAlignment = RenderingLibrary.Graphics.HorizontalAlignment;
using Orientation = Gum.Forms.Controls.Orientation;

namespace DLLevelBuilder.ProjectFeatures;

public class LevelManagerPopup : Popup<LevelManagerPopup>
{
    private static readonly float scrollPadding = 16f;
    
    private readonly ScrollViewer scrollViewer;
    private readonly StackPanel panel;
    private readonly ColoredRectangleRuntime popupBG;
    private readonly RectangleRuntime popupBGBorder;

    private List<LevelVisual> visuals = [];
    private readonly Texture2D closeIcon;

    private class LevelVisual
    {
        public int? id;
        public Level? levelData;
        private readonly ContainerRuntime panel;
        private readonly TextRuntime nameText;
        private readonly Button removeButton;
        private readonly ColoredRectangleRuntime background;
        private readonly ContainerRuntime innerPanel;

        public LevelVisual(Texture2D closeIcon, StackPanel parent, int? id = null, Level? levelData = null)
        {
            this.id = id;
            this.levelData = levelData;
            
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
                X = 4,
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
            if (this.id == null || this.levelData == null)
                return;
            this.panel.Visible = true;
        }

        public void RemoveAndHide()
        {
            this.panel.Visible = false;
            // if (this.id != null)
            //     Program.instance.cmdHistory.ApplyCmd(new RemoveEntityDataCmd(Project.instance, this.id, OnUndoRedo)); TODO
            this.id = null;
            this.levelData = null;
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
    }
    
    public LevelManagerPopup()
    {
        this.closeIcon = Program.instance.Content.Load<Texture2D>("Icons/Cross");
        
        this.scrollViewer = new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden };
        this.panel = new StackPanel
        {
            Visual =
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                Width = 100,
                Height = scrollPadding
            },
            Y = scrollPadding,
            Spacing = scrollPadding
        };
        this.popupBG = new ColoredRectangleRuntime { Color = Params.DefaultFillColor };
        this.popupBGBorder = new RectangleRuntime { Color = Params.DefaultOutlineColor };

        ScrollViewerVisual scrollViewerVisual = (ScrollViewerVisual)this.scrollViewer.Visual;
        scrollViewerVisual.Background.Color = Microsoft.Xna.Framework.Color.Transparent;
        ScrollBarVisual scrollBarVisual = scrollViewerVisual.VerticalScrollBarInstance;
        scrollBarVisual.Width = Params.minBoxSize * 0.2f;
        scrollBarVisual.DownButtonIcon.Visible = false;
        scrollBarVisual.UpButtonIcon.Visible = false;
        CustomButtonVisual.Create(scrollBarVisual.UpButtonInstance, CustomButtonVisual.CustomButtonTheme.InvertedTheme);
        CustomButtonVisual.Create(scrollBarVisual.DownButtonInstance, CustomButtonVisual.CustomButtonTheme.InvertedTheme);
        CustomButtonVisual.Create(scrollBarVisual.ThumbInstance, CustomButtonVisual.CustomButtonTheme.InvertedTheme);
        
        
        this.container.AddChild(this.popupBG);
        this.container.AddChild(this.popupBGBorder);
        this.container.AddChild(this.scrollViewer.Visual);
        this.scrollViewer.AddChild(this.panel);

        Project.Instance.RegisterOnLevelsChanged(LoadLevels);
        
        UpdatePositionsAndSizes();
        LoadLevels(Project.Instance.levels);
    }

    protected override void UpdatePositionsAndSizes()
    {
        base.UpdatePositionsAndSizes();

        const float popupWidth = 400f;
        const float popupHeight = 300f;
        const float margin = 16f;

        float containerWidth = this.popUpContainerRef.Width;

        // Top-right anchor
        float popupX = containerWidth - popupWidth - margin;
        const float popupY = margin + Params.minBoxSize;

        // Background
        this.popupBG.Width = popupWidth;
        this.popupBG.Height = popupHeight;
        this.popupBG.X = popupX;
        this.popupBG.Y = popupY;

        // Background border
        this.popupBGBorder.Width = popupWidth + Params.defaultOutLineWidth;
        this.popupBGBorder.Height = popupHeight + Params.defaultOutLineWidth;
        this.popupBGBorder.X = popupX - Params.defaultOutLineWidth / 2;
        this.popupBGBorder.Y = popupY - Params.defaultOutLineWidth / 2;

        // ScrollPanel
        ScrollViewerVisual scrollViewerVisual = (ScrollViewerVisual)this.scrollViewer.Visual;
        ScrollBarVisual scrollBarVisual = scrollViewerVisual.VerticalScrollBarInstance;
        scrollBarVisual.Width = Params.minBoxSize * 0.3f;
        
        this.scrollViewer.Width = popupWidth - Params.popupPadding + scrollBarVisual.Width * 0.5f;
        this.scrollViewer.Height = popupHeight - Params.popupPadding * 1.5f + scrollBarVisual.Width * 0.5f - Params.minBoxSize;
        this.scrollViewer.X = popupX + Params.popupPadding / 2;
        this.scrollViewer.Y = popupY + Params.popupPadding + Params.minBoxSize;
        
        // instances
        foreach (LevelVisual levelVisual in this.visuals) levelVisual.UpdateVisuals();
    }
    
    private void LoadLevels(IReadOnlyList<Level> data)
    {
        List<LevelVisual> upToDateVisuals = [];
        for (int id = 0; id < data.Count; id++)
        {
            Level level = data[id];
            
            if (level == null)
                continue;
            LevelVisual? instance = this.visuals.FirstOrDefault(x => x.id == id);
            if (instance != null)
            {
                instance.UpdateVisuals();
                upToDateVisuals.Add(instance);
                continue;
            }

            instance = this.visuals.FirstOrDefault(x => x.id == null);
            if (instance == null)
            {
                instance = new LevelVisual(this.closeIcon, this.panel, id, level);
                this.visuals.Add(instance);
                upToDateVisuals.Add(instance);
                continue;
            }

            instance.levelData = level;
            instance.id = id;
            instance.UpdateVisuals();
            upToDateVisuals.Add(instance);
        }

        foreach (LevelVisual entityDataVisual in this.visuals.Where(entityDataVisual => !upToDateVisuals.Contains(entityDataVisual)))
            entityDataVisual.RemoveAndHide();
    }
}