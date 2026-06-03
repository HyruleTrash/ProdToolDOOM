using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures;

public class NewLevelFeature(Project project) : ProjectFeature
{
    private MenuItem toggleManagerButton = null!;

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;
        
        this.toggleManagerButton = new MenuItem
        {
            Header = "New",
            Height = UIParams.minButtonHeight
        };
        // UIParams.SetDefaultButton(this.switchLeft);
        this.toggleManagerButton.Clicked += (_, _) => AddLevel();
        
        this.children.Add(this.toggleManagerButton);
        SetVisible(isVisible);
    }
    
    private void AddLevel() => Program.instance.cmdHistory.ApplyCmd(new AddLevelCmd(project));
}