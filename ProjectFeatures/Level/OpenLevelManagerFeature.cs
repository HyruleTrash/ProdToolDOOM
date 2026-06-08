using DLLevelBuilder.UI;
using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures;

public class OpenLevelManagerFeature : ProjectFeature
{
    private MenuItem toggleManagerButton = null!;

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;
        
        this.toggleManagerButton = new MenuItem
        {
            Header = "Manage",
            Height = Params.minButtonHeight
        };
        CustomMenuItemVisual.Create(this.toggleManagerButton);
        this.toggleManagerButton.Clicked += (_, _) => TogglePopup();
        
        this.children.Add(this.toggleManagerButton);
        SetVisible(isVisible);
    }
    
    //private void AddLevel() => Program.instance.cmdHistory.ApplyCmd(new AddLevelCmd(project));
    
    private static void TogglePopup() => LevelManagerPopup.ToggleVisibility();
}