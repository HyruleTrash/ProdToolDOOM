using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures;

public class SaveFeature(Project project) : ProjectFeature
{
    private MenuItem saveProjectButton = null!;

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;

        this.saveProjectButton = new MenuItem
        {
            Header = "Save Project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultMenuItem(this.saveProjectButton);

        this.saveProjectButton.Clicked += (_, _) => Save();
        this.children.Add(this.saveProjectButton);
        SetVisible(isVisible);
    }

    private void Save() => Program.instance.cmdHistory.ApplyCmd(new SaveProjectCmd(project));
}