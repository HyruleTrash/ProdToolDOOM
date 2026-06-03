namespace DLLevelBuilder;

public class SaveProjectCmd(Project projectRef) : ICommand
{
    public delegate bool ShouldOverwriteDelegate(ref string filePath);
    public ShouldOverwriteDelegate shouldOverwriteFilePath = (ref string _) => projectRef.FilePath != string.Empty;

    public void Execute()
    {
        if (projectRef.CheckSaveStrategy())
            return;
        Debug.Log("Saving project file...");

        string tempPath = projectRef.FilePath;
        if (!this.shouldOverwriteFilePath.Invoke(ref tempPath))
            return;

        projectRef.Save(tempPath);
    }

    public void Undo()
    {
        // Is this really needed?
    }
}