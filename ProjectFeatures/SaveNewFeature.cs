using Gum.Forms.DefaultVisuals;
using Button = Gum.Forms.Controls.Button;
using static Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM.ProjectFeatures;

public class SaveNewFeature : SaveFeature
{
    private const string projectFileFilter = "wapd files (*.wapd)|*.wapd";
    private Button saveProjectAsButton = null!;

    public SaveNewFeature(Project project) : base(project)
    {
        this.shouldOverwriteFilePath = ShouldOverwriteFilePath;
    }
    
    public override void LoadUI(object? parent)
    {
        if (!ShouldLoadUI(parent))
            return;

        this.saveProjectAsButton = new Button
        {
            Text = "New Project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.saveProjectAsButton);

        this.saveProjectAsButton.Click += (_, _) => Save();
        this.project.filePathChanged += (newPath) =>
        {
            this.saveProjectAsButton.Text = newPath == string.Empty ? "New Project" : "Save Project as...";
        };
        AddUI(parent, this.saveProjectAsButton);
    }

    private bool ShouldOverwriteFilePath(ref string tempPath)
    {
        FileExplorerHelper.FileDialogResult? result = this.project.FilePath == string.Empty
            ? FileExplorerHelper.SaveWithFileExplorer(projectFileFilter)
            : FileExplorerHelper.SaveWithFileExplorer(projectFileFilter, tempPath);

        if (!result.HasValue)
            return false;
        tempPath = result.Value.filePath;

        return true;
    }
}