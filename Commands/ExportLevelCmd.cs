using System.Text;
using DLLevelBuilder.ProjectFeatures.Exporting;

namespace DLLevelBuilder;

public class ExportLevelCmd(Project project, Level level) : ICommand
{
    private static readonly ExportOption[] ExportOptions =
    [
        new FbxExport(),
        new ObjExport(),
        new JsonExport()
    ];

    private static string GetFilters()
    {
        StringBuilder sb = new();
        for (int i = 0; i < ExportOptions.Length; i++)
        {
            ExportOption exportOption = ExportOptions[i];
            if (i != 0)
                sb.Append('|');
            sb.Append(exportOption.GetFilter());
        }

        return sb.ToString();
    }
    
    public void Execute()
    {
        if (level == null || project == null) return;
        
        FileExplorerHelper.FileDialogResult? result = FileExplorerHelper.SaveWithFileExplorer(GetFilters());
        if (!result.HasValue) return;

        try
        {
            bool exportResult = false;
            foreach (ExportOption exportOption in ExportOptions)
            {
                if (!exportOption.CheckExtension(result.Value.fileExtension)) continue;
                exportResult = exportOption.Export(result.Value.filePath, level);
                break;
            }

            if (!exportResult)
                Debug.Log($"Failed to save file {result.Value.filePath}");
            else
                Debug.Log($"Successfully saved file {result.Value.filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void Undo()
    {
        // Is this really needed?
    }
}