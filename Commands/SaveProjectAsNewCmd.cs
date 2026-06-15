namespace DLLevelBuilder;

public class SaveProjectAsNewCmd(Project projectRef) : ICommand
{
    private const string ProjectFileFilter = "wapd files (*.wapd)|*.wapd";
    
    public void Execute()
    {
        SaveProjectCmd cmd = new(projectRef) { shouldOverwriteFilePath = ShouldOverwriteFilePath };
        cmd.Execute();
    }

    public void Undo()
    {
        // unused
    }
    
    private bool ShouldOverwriteFilePath(ref string tempPath)
    {
        FileExplorerHelper.FileDialogResult? result = projectRef.FilePath == string.Empty
            ? FileExplorerHelper.SaveWithFileExplorer(ProjectFileFilter)
            : FileExplorerHelper.SaveWithFileExplorer(ProjectFileFilter, tempPath);

        if (!result.HasValue)
            return false;
        tempPath = result.Value.filePath;

        return true;
    }
}