using DLLevelBuilder.UI;
using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures;

public class NewLevelFeature(Project project) : ProjectFeature
{
    private MenuItem createNewButton = null!;

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;
        
        this.createNewButton = new MenuItem
        {
            Header = "New",
            Height = Params.minButtonHeight
        };
        CustomMenuItemVisual.Create(this.createNewButton);
        this.createNewButton.Clicked += (_, _) => AddLevel();
        
        this.children.Add(this.createNewButton);
        SetVisible(isVisible);
    }
    
    private void AddLevel() => Program.instance.cmdHistory.ApplyCmd(new AddLevelCmd(project));
}