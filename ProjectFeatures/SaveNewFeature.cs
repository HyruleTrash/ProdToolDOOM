using Gum.Forms.Controls;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.ProjectFeatures;

public class SaveNewFeature : SaveFeature
{
    private const string projectFileFilter = "wapd files (*.wapd)|*.wapd";
    private MenuItem saveProjectAsButton = null!;

    public SaveNewFeature(Project project) : base(project)
    {
        this.shouldOverwriteFilePath = ShouldOverwriteFilePath;
    }
    
    public override void LoadUI(MenuItem menu)
    {
        if (!ShouldLoadUI(menu))
            return;

        this.saveProjectAsButton = new MenuItem
        {
            Header = "New Project",
            Height = UIParams.minButtonHeight
        };
        // UIParams.SetDefaultButton(this.saveProjectAsButton);

        this.saveProjectAsButton.Clicked += (_, _) => Save();
        this.project.filePathChanged += (newPath) =>
        {
            this.saveProjectAsButton.Header = newPath == string.Empty ? "New Project" : "Save Project as...";
        };
        
        menu.Items.Add(this.saveProjectAsButton);
    }
    
    public override void SetVisible(bool state) => this.saveProjectAsButton.IsVisible = state;

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