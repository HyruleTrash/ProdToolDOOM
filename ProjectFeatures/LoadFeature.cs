using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using MonoGameGum;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures;

public class LoadFeature(Project project) : ProjectFeature
{
    private Button loadProjectButton = null!;

    public override void LoadUI(object parent)
    {
        if (!ShouldLoadUI(parent))
            return;

        this.loadProjectButton = new Button
        {
            Text = "Load Project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.loadProjectButton);

        this.loadProjectButton.Click += (sender, args) => Load();
        AddUI(parent, this.loadProjectButton);
    }

    private void Load()
    {
        if (project.CheckLoadStrategy())
            return;
        Debug.Log("Loading project file...");

        string tempPath = project.FilePath;
        FileExplorerHelper.FileDialogResult? result = tempPath == string.Empty
            ? FileExplorerHelper.OpenFileExplorer()
            : FileExplorerHelper.OpenFileExplorer(tempPath);
        if (!result.HasValue)
            return;
        tempPath = result.Value.filePath;

        if (project.loadStrat.Load(tempPath))
            project.FilePath = tempPath;
    }
}