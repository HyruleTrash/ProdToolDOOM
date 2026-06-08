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
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultMenuItem(this.switchLeft);
        this.switchLeft.Clicked += (_, _) => SwitchLevel(-1);
        
        this.switchRight = new MenuItem
        {
            Header = "Next",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultMenuItem(this.switchRight);
        this.switchRight.Clicked += (_, _) => SwitchLevel(1);
        
        this.children.Add(this.switchLeft);
        this.children.Add(this.switchRight);
        SetVisible(isVisible);
    }

    private void SwitchLevel(int direction)
    {
        project.CurrentLevel += direction;
        Debug.Log($"Switched level {project.CurrentLevel}");
        Program.instance.cmdHistory.ApplyCmd(new SaveProjectCmd(project));
        if (project.CheckLoadStrategy())
            return;
        Debug.Log("Reloading project file...");
        project.Load(project.FilePath);
    }
}