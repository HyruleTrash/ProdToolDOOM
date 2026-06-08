using System;
using System.Text;
using DLLevelBuilder.UI;
using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures.Exporting;

public class ExportFeature : ProjectFeature
{
    private readonly Project projectRef;
    private readonly ExportOption[] exportOptions;
    private MenuItem exportButton = null!;

    public ExportFeature(Project project)
    {
        this.projectRef = project;
        this.exportOptions =
        [
            new FbxExport(),
            new ObjExport(),
            new JsonExport()
        ];
    }

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;
        
        this.exportButton = new MenuItem
        {
            Header = "Export Level",
            Height = Params.minButtonHeight
        };
        CustomMenuItemVisual.Create(this.exportButton);
        this.exportButton.Clicked += (_, _) => Export();
        this.children.Add(this.exportButton);
        SetVisible(isVisible);
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