using Gum.Forms.Controls;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.ProjectFeatures;

public class LoadFeature(Project project) : ProjectFeature
{
    private MenuItem loadProjectButton = null!;

    public override void LoadUI(MenuItem menu)
    {
        if (!ShouldLoadUI(menu))
            return;

        this.loadProjectButton = new MenuItem
        {
            Header = "Load Project",
            Height = UIParams.minButtonHeight
        };
        // UIParams.SetDefaultButton(this.loadProjectButton);

        this.loadProjectButton.Clicked += (_, _) => Load();
        menu.Items.Add(this.loadProjectButton);
    }
    
    public override void SetVisible(bool state) => this.loadProjectButton.IsVisible = state;

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