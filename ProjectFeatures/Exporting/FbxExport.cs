using Assimp;
namespace ProdToolDOOM.ProjectFeatures.Exporting;

public class FbxExport : ExportOption
{
    public FbxExport() : base("Fbx files", ".fbx") { }
    
    public override bool Export(string filePath)
    {
        try
        {
            using AssimpContext context = new();
            Scene scene = new() { RootNode = new Node("RootNode") };
            
            Material defaultMat = new() { Name = "DefaultMaterial" };
            scene.Materials.Add(defaultMat);

            Mesh exportMesh = GenerateMesh();
            exportMesh.MaterialIndex = 0;
                
            scene.Meshes.Add(exportMesh);
            scene.RootNode.MeshIndices.Add(0);
            
            Node meshNode = new("LevelMeshObject");
            meshNode.MeshIndices.Add(scene.Meshes.Count - 1);
            scene.RootNode.Children.Add(meshNode);

            return context.ExportFile(scene, filePath, "fbx");
        }
        catch (Exception e)
        {
            Debug.LogError($"Fbx export failed: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    private Mesh GenerateMesh()
    {
        Mesh mesh = new("LevelMesh", PrimitiveType.Triangle);

        // Example: Creating a simple triangle
        const float scaleFactor = 100.0f; 
        // Vertices
        mesh.Vertices.Add(new Vector3D(0, 0, 0) * scaleFactor);
        mesh.Vertices.Add(new Vector3D(1, 0, 0) * scaleFactor);
        mesh.Vertices.Add(new Vector3D(0, 1, 0) * scaleFactor);

        // Normals
        mesh.Normals.Add(new Vector3D(0, 0, 1));
        mesh.Normals.Add(new Vector3D(0, 0, 1));
        mesh.Normals.Add(new Vector3D(0, 0, 1));

        // Faces (Indices)
        mesh.Faces.Add(new Face([0, 1, 2]));

        return mesh;
    }
}