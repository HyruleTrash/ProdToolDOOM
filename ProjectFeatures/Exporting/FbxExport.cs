using Assimp;
namespace ProdToolDOOM.ProjectFeatures.Exporting;

public class FbxExport : ExportOption
{
    public FbxExport() : base("Fbx files", ".fbx") { }
    public override bool Export(string filePath, Level level) => AssimpExport.Export(filePath, "fbx", level);
}