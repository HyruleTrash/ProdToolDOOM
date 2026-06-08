
using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures;

public class OpenEntityDataManageFeature : ProjectFeature
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
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultMenuItem(this.toggleManagerButton);
        this.toggleManagerButton.Clicked += (_, _) => TogglePopup();
        
        this.children.Add(this.toggleManagerButton);
        SetVisible(isVisible);
    }

    private static void TogglePopup() => EntityManagerPopup.ToggleVisibility();
}