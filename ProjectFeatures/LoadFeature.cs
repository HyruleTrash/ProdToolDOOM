using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using MonoGameGum;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures;

public class LoadFeature(Project project) : ProjectFeature
{
    private Button loadProjectButton = null!;

    public override void LoadUI(object? parent)
    {
        if (!ShouldLoadUI(parent))
            return;
        
        loadProjectButton = new Button
        {
            Text = "Load Project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton((ButtonVisual)loadProjectButton.Visual);
        
        loadProjectButton.Click += (sender, args) => Load();
        AddUI(parent, loadProjectButton);
    }

    private void Load()
    {
        if (project.CheckLoadStrategy())
            return;
        Debug.Log("Loading project file...");

        var tempPath = project.FilePath;
        if (tempPath == string.Empty)
        {
            tempPath = FileExplorerHelper.OpenFileExplorer();
            Debug.Log(tempPath);
        }
        else
            tempPath = FileExplorerHelper.OpenFileExplorer(tempPath);

        if (project.loadStrat.Load(tempPath))
            project.FilePath = tempPath;
    }
}