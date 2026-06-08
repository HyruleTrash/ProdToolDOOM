using DLLevelBuilder.UI;
using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures;

public class LoadFeature(Project project) : ProjectFeature
{
    private MenuItem loadProjectButton = null!;

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;

        this.loadProjectButton = new MenuItem
        {
            Header = "Load Project",
            Height = Params.minButtonHeight
        };
        CustomMenuItemVisual.Create(this.loadProjectButton);

        this.loadProjectButton.Clicked += (_, _) => Load();
        this.children.Add(this.loadProjectButton);
        SetVisible(isVisible);
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

        project.Load(tempPath);
    }
}