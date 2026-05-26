using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;
using ComboBox = Gum.Forms.Controls.ComboBox;

namespace DLLevelBuilder.ProjectFeatures;

public class EntitySetIdPopup : Popup<EntitySetIdPopup>
{
    private readonly StackPanel panel;
    private readonly ComboBox dropDownBox;
    private readonly ColoredRectangleRuntime popupBG;
    private readonly RectangleRuntime popupBGBorder;
    private Button confirmButton;
    private Entity? currentSelected;

    private List<Option> options = [];

    private class Option(int idInComboBox, int entityId, string name)
    {
        public int idInComboBox = idInComboBox;
        public int entityId = entityId;
        public string name = name;
    }
    
    public EntitySetIdPopup()
    {
        this.panel = new StackPanel { Spacing = 5, };
        this.popupBG = new ColoredRectangleRuntime { Color = UIParams.defaultFillColor };
        this.popupBGBorder = new RectangleRuntime { Color = UIParams.defaultOutlineColor };

        this.dropDownBox = new ComboBox()
        {
            Width = 200,
            Height = 40,
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
        this.panel.AddChild(this.dropDownBox);
        this.panel.AddChild(this.confirmButton);

        Project.instance.onEntityDataChanged += LoadPossibleEntities;
        
        UpdatePositionsAndSizes();
        LoadPossibleEntities(Project.instance.EntityDatas);
    }

    private void LoadPossibleEntities(IReadOnlyDictionary<int, EntityData> entityDatas)
    {
        List<Option> newList = entityDatas.Select(keyValuePair => new Option(-1, keyValuePair.Key, keyValuePair.Value.Name)).ToList();
        newList.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
        
        this.dropDownBox.Items.Clear();
        for (int i = 0; i < newList.Count; i++)
        {
            Option option = newList[i];
            option.idInComboBox = i;
            this.dropDownBox.Items.Add(option.name);
            newList[i] = option;
        }
        
        this.options = newList;
    }

    private void ConfirmCreation()
    {
        if (this.currentSelected == null) return;
        try
        {
            Program.instance.cmdHistory.ApplyCmd(new ChangeEntityDataRef(Project.instance, this.currentSelected, GetSelectedId(this.dropDownBox.SelectedIndex)));
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            this.dropDownBox.SelectedIndex = -1;
            this.currentSelected = null;
            ToggleVisibility();
        }
    }

    private int? GetSelectedId(int selectedIndex) => this.options.FirstOrDefault(x => x.idInComboBox == selectedIndex)?.entityId;

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

    public void SetCurrentSelection(Entity entity)
    {
        this.currentSelected = entity;
        Option? found = this.options.FirstOrDefault(x => x.entityId == entity.DataId);
        if (found != null)
            this.dropDownBox.SelectedIndex = found.idInComboBox;
    }
}