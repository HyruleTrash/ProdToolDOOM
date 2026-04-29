namespace ProdToolDOOM.ProjectFeatures.Exporting;

public class ObjExport : ExportOption
{
    public ObjExport() : base("Obj files", ".obj") { }
    public override bool Export(string filePath, Level level) => AssimpExport.Export(filePath, "obj", level);
}