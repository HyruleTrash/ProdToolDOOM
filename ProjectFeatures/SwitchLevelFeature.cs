using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures;

public class SwitchLevelFeature : SaveFeature
{
    private ContainerRuntime container;
    private Button switchLeft;
    private Button switchRight;

    public SwitchLevelFeature(Project project) : base(project) { }

    public override void LoadUI(object parent)
    {
        if (!ShouldLoadUI(parent))
            return;

        this.container = new ContainerRuntime()
        {
            Height = UIParams.minBoxSize,
            HasEvents = false
        };
        
        this.switchLeft = new Button
        {
            Text = "<",
            Width = UIParams.minBoxSize,
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.switchLeft);
        this.switchLeft.Click += (_, _) => SwitchLevel(-1);
        AddUI(this.container, this.switchLeft);
        
        this.switchRight = new Button
        {
            Text = ">",
            X = UIParams.minBoxSize,
            Width = UIParams.minBoxSize,
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.switchRight);
        this.switchRight.Click += (_, _) => SwitchLevel(1);
        AddUI(this.container, this.switchRight);

        AddUI(parent, this.container);
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