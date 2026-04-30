using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;
using Orientation = Gum.Forms.Controls.Orientation;

namespace ProdToolDOOM.ProjectFeatures;

public class EntityManagerPopup : Popup<EntityManagerPopup>
{
    private readonly StackPanel panel;
    private readonly ColoredRectangleRuntime popupBG;
    private readonly RectangleRuntime popupBGBorder;

    private List<EntityDataVisual> visuals = [];
    private readonly Texture2D closeIcon;

    private class EntityDataVisual
    {
        public int? id;
        public EntityData? entityData;
        private readonly StackPanel panel;
        private readonly TextRuntime nameText;
        private readonly Button removeButton;

        public EntityDataVisual(Texture2D closeIcon, StackPanel parent, int? id = null, EntityData? entityData = null)
        {
            this.id = id;
            this.entityData = entityData;
            
            Debug.Log($"Instantiating EntityDataVisual {this.id}");
            
            // instantiate visuals
            this.panel = new StackPanel
            {
                Spacing = 5,
                Width = 400f,
                Orientation = Orientation.Horizontal
            };

            this.nameText = new TextRuntime
            {
                Text = entityData?.Name ?? "Unnamed",
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
            if (this.id == null || this.entityData == null)
                return;
            this.panel.IsVisible = true;
            this.nameText.Text = this.entityData.Name;
        }

        public void RemoveAndHide()
        {
            this.panel.IsVisible = false;
            if (this.id != null)
                Program.instance.cmdHistory.ApplyCmd(new RemoveEntityDataCmd(Project.instance, this.id, OnUndoRedo));
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
    
    public EntityManagerPopup()
    {
        this.closeIcon = Program.instance.Content.Load<Texture2D>("Icons/Cross");
        
        this.panel = new StackPanel { Spacing = 5, Orientation = Orientation.Vertical };
        this.popupBG = new ColoredRectangleRuntime { Color = UIParams.defaultFillColor };
        this.popupBGBorder = new RectangleRuntime { Color = UIParams.defaultOutlineColor };
        
        this.container.AddChild(this.popupBG);
        this.container.AddChild(this.popupBGBorder);
        this.container.AddChild(this.panel.Visual);

        Project.instance.onEntityDataChanged += LoadEntityData;
        
        UpdatePositionsAndSizes();
        LoadEntityData(Project.instance.EntityDatas);
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
    
    private void LoadEntityData(IReadOnlyDictionary<int, EntityData> data)
    {
        List<EntityDataVisual> upToDateVisuals = [];
        foreach (KeyValuePair<int, EntityData> keyValuePair in data)
        {
            if (keyValuePair.Value == null)
                continue;
            EntityDataVisual? instance = this.visuals.FirstOrDefault(x => x.id == keyValuePair.Key);
            if (instance != null)
            {
                instance.UpdateVisuals();
                upToDateVisuals.Add(instance);
                continue;
            }

            instance = this.visuals.FirstOrDefault(x => x.id == null);
            if (instance == null)
            {
                instance = new EntityDataVisual(this.closeIcon, this.panel, keyValuePair.Key, keyValuePair.Value);
                this.visuals.Add(instance);
                upToDateVisuals.Add(instance);
                continue;
            }

            instance.entityData = keyValuePair.Value;
            instance.id = keyValuePair.Key;
            instance.UpdateVisuals();
            upToDateVisuals.Add(instance);
        }
        
        foreach (EntityDataVisual entityDataVisual in this.visuals.Where(entityDataVisual => !upToDateVisuals.Contains(entityDataVisual)))
            entityDataVisual.RemoveAndHide();
    }
}