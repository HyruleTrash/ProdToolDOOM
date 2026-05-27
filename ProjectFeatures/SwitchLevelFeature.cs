using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.ProjectFeatures;

public class SwitchLevelFeature : SaveFeature
{
    private ContainerRuntime container;
    private MenuItem switchLeft;
    private MenuItem switchRight;

    public SwitchLevelFeature(Project project) : base(project) { }

    public override void LoadUI(MenuItem menu)
    {
        if (!ShouldLoadUI(menu))
            return;
        
        this.switchLeft = new MenuItem
        {
            Header = "Previous",
            Height = UIParams.minButtonHeight
        };
        // UIParams.SetDefaultButton(this.switchLeft);
        this.switchLeft.Clicked += (_, _) => SwitchLevel(-1);
        menu.Items.Add(this.switchLeft);
        
        this.switchRight = new MenuItem
        {
            Header = "Next",
            Height = UIParams.minButtonHeight
        };
        // UIParams.SetDefaultButton(this.switchRight);
        this.switchRight.Clicked += (_, _) => SwitchLevel(1);
        menu.Items.Add(this.switchRight);
    }
    
    public override void SetVisible(bool state)
    {
        this.switchLeft.IsVisible = state;
        this.switchRight.IsVisible = state;
    }

    private void SwitchLevel(int direction)
    {
        this.project.CurrentLevel += direction;
        Debug.Log($"Switched level {this.project.CurrentLevel}");
        Save();
        if (this.project.CheckLoadStrategy())
            return;
        Debug.Log("Reloading project file...");
        project.loadStrat.Load(this.project.FilePath);
    }
}