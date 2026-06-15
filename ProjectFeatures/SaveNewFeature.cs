using DLLevelBuilder.UI;
using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures;

public class SaveNewFeature(Project project) : ProjectFeature
{
    private MenuItem saveProjectAsButton = null!;

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;

        this.saveProjectAsButton = new MenuItem
        {
            Header = "New Project",
            Height = Params.minButtonHeight
        };
        CustomMenuItemVisual.Create(this.saveProjectAsButton);

        this.saveProjectAsButton.Clicked += (_, _) => Save();
        project.filePathChanged += newPath =>
        {
            this.saveProjectAsButton.Header = newPath == string.Empty ? "New Project" : "Save Project as...";
        };
        
        this.children.Add(this.saveProjectAsButton);
        SetVisible(isVisible);
    }

    private void Save() => Program.instance.cmdHistory.ApplyCmd(new SaveProjectAsNewCmd(project));
}