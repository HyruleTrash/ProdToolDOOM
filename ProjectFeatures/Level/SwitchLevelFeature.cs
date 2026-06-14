using DLLevelBuilder.UI;
using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures;

public class SwitchLevelFeature(Project project) : ProjectFeature
{
    private MenuItem switchLeft = null!;
    private MenuItem switchRight = null!;

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;
        
        this.switchLeft = new MenuItem
        {
            Header = "Previous",
            Height = Params.minButtonHeight
        };
        CustomMenuItemVisual.Create(this.switchLeft);
        this.switchLeft.Clicked += (_, _) => SwitchLevel(-1);
        
        this.switchRight = new MenuItem
        {
            Header = "Next",
            Height = Params.minButtonHeight
        };
        CustomMenuItemVisual.Create(this.switchRight);
        this.switchRight.Clicked += (_, _) => SwitchLevel(1);
        
        this.children.Add(this.switchLeft);
        this.children.Add(this.switchRight);
        SetVisible(isVisible);
    }

    private void SwitchLevel(int direction) => Program.instance.cmdHistory.ApplyCmd(new SwitchLevelCmd(project, direction));
}