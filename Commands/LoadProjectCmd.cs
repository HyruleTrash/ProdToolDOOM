namespace DLLevelBuilder;

public class LoadProjectCmd(Project projectRef) : ICommand
{
    public void Execute()
    {
        if (projectRef.CheckLoadStrategy())
            return;
        Debug.Log("Loading project file...");

        string tempPath = projectRef.FilePath;
        FileExplorerHelper.FileDialogResult? result = tempPath == string.Empty
            ? FileExplorerHelper.OpenFileExplorer()
            : FileExplorerHelper.OpenFileExplorer(tempPath);
        if (!result.HasValue)
            return;
        tempPath = result.Value.filePath;

        projectRef.Load(tempPath);
    }

    public void Undo()
    {
        // unused
    }
}