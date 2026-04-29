namespace ProdToolDOOM.ProjectFeatures.Exporting;

public abstract class ExportOption(string pretext, string extension)
{
    public string GetFilter() => $"{pretext} (*{extension})|*{extension}";
    public bool CheckExtension(string extensionToCheck) => extensionToCheck == extension;
    public abstract bool Export(string valueFilePath, Level level);
}