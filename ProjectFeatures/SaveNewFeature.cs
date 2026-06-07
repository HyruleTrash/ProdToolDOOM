using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures;

public class SaveNewFeature(Project project) : ProjectFeature
{
    private const string ProjectFileFilter = "wapd files (*.wapd)|*.wapd";
    private MenuItem saveProjectAsButton = null!;

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;

        this.saveProjectAsButton = new MenuItem
        {
            Header = "New Project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultMenuItem(this.saveProjectAsButton);

        this.saveProjectAsButton.Clicked += (_, _) => Save();
        project.filePathChanged += newPath =>
        {
            this.saveProjectAsButton.Header = newPath == string.Empty ? "New Project" : "Save Project as...";
        };
        
        this.children.Add(this.saveProjectAsButton);
        SetVisible(isVisible);
    }

    private void Save()
    {
        SaveProjectCmd cmd = new(project) { shouldOverwriteFilePath = ShouldOverwriteFilePath };
        Program.instance.cmdHistory.ApplyCmd(cmd);
    }

    private bool ShouldOverwriteFilePath(ref string tempPath)
    {
        FileExplorerHelper.FileDialogResult? result = project.FilePath == string.Empty
            ? FileExplorerHelper.SaveWithFileExplorer(ProjectFileFilter)
            : FileExplorerHelper.SaveWithFileExplorer(ProjectFileFilter, tempPath);

        if (!result.HasValue)
            return false;
        tempPath = result.Value.filePath;

        return true;
    }
}