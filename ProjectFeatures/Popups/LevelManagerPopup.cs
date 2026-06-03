using System.Collections.Generic;
using System.Linq;
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
                Width = UIParams.minBoxSize,
                Height = UIParams.minBoxSize
            };
            UIParams.SetDefaultButton(this.removeButton);
            UIParams.AddIconToButton(this.removeButton, closeIcon);
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
        this.popupBG = new ColoredRectangleRuntime { Color = UIParams.defaultFillColor };
        this.popupBGBorder = new RectangleRuntime { Color = UIParams.defaultOutlineColor };
        
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
        const float popupY = margin + UIParams.minBoxSize;

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