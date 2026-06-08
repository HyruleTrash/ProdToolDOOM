using System.Collections.Generic;
using System.Linq;
using DLLevelBuilder.UI;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;
using Orientation = Gum.Forms.Controls.Orientation;

namespace DLLevelBuilder.ProjectFeatures;

public class LevelManagerPopup : Popup<LevelManagerPopup>
{
    private readonly ScrollViewer panel;
    private readonly ColoredRectangleRuntime popupBG;
    private readonly RectangleRuntime popupBGBorder;

    private List<LevelVisual> visuals = [];
    private readonly Texture2D closeIcon;

    private class LevelVisual
    {
        public int? id;
        public Level? levelData;
        private readonly StackPanel panel;
        private readonly TextRuntime nameText;
        private readonly Button removeButton;

        public LevelVisual(Texture2D closeIcon, ScrollViewer parent, int? id = null, Level? levelData = null)
        {
            this.id = id;
            this.levelData = levelData;
            
            // instantiate visuals
            this.panel = new StackPanel
            {
                Spacing = 5,
                Width = 400f,
                Orientation = Orientation.Horizontal
            };

            this.nameText = new TextRuntime
            {
                Text = $"Level {this.id}",
                Width = 200f
            };

            this.removeButton = new Button
            {
                Text = "X",
                Width = Params.minBoxSize,
                Height = Params.minBoxSize
            };
            CustomButtonVisual.Create(this.removeButton);
            CustomButtonVisual.AddIcon(this.removeButton, closeIcon);
            this.removeButton.Click += (_, _) => RemoveAndHide();

            this.panel.AddChild(this.nameText);
            this.panel.AddChild(this.removeButton);
            parent.AddChild(this.panel);
        }

        public void UpdateVisuals()
        {
            if (this.id == null || this.levelData == null)
                return;
            this.panel.IsVisible = true;
        }

        public void RemoveAndHide()
        {
            this.panel.IsVisible = false;
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
        
        this.panel = new ScrollViewer { InnerPanel = { StackSpacing = 4 } };
        this.popupBG = new ColoredRectangleRuntime { Color = Params.DefaultFillColor };
        this.popupBGBorder = new RectangleRuntime { Color = Params.DefaultOutlineColor };
        
        this.container.AddChild(this.popupBG);
        this.container.AddChild(this.popupBGBorder);
        this.container.AddChild(this.panel.Visual);

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

        // Panel
        this.panel.Width = popupWidth - Params.popupPadding;
        this.panel.Height = popupHeight - Params.popupPadding;
        this.panel.X = popupX + Params.popupPadding / 2;
        this.panel.Y = popupY + Params.popupPadding / 2;
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