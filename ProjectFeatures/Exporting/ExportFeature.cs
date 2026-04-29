using System.Text;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures.Exporting;

public class ExportFeature : ProjectFeature
{
    private readonly Project projectRef;
    private readonly ExportOption[] exportOptions;
    private Button exportButton = null!;

    public ExportFeature(Project project)
    {
        this.projectRef = project;
        this.exportOptions =
        [
            new FbxExport(),
            new ObjExport()
        ];
    }

    public override void LoadUI(object parent)
    {
        if (!ShouldLoadUI(parent))
            return;
        
        this.exportButton = new Button
        {
            Text = "Export Level",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.exportButton);
        this.exportButton.Click += (_, _) => Export();

        AddUI(parent, this.exportButton);
    }

    private string GetFilters()
    {
        StringBuilder sb = new();
        for (int i = 0; i < this.exportOptions.Length; i++)
        {
            ExportOption exportOption = this.exportOptions[i];
            if (i != 0)
                sb.Append('|');
            sb.Append(exportOption.GetFilter());
        }

        return sb.ToString();
    }

    private void Export()
    {
        FileExplorerHelper.FileDialogResult? result = FileExplorerHelper.SaveWithFileExplorer(GetFilters());
        if (!result.HasValue)
            return;

        Level level = this.projectRef.levels[this.projectRef.CurrentLevel];
        
        try
        {
            bool exportResult = false;
            foreach (ExportOption exportOption in this.exportOptions)
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
}