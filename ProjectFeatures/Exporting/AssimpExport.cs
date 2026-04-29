using Assimp;

namespace ProdToolDOOM.ProjectFeatures.Exporting;

public static class AssimpExport
{
    private const float ScaleFactor = 100.0f; 
    
    private class FlatPlane(Line[] points, Vector3D normal)
    {
        public Line[] points = points;
        public Vector3D normal = normal;
    }
    
    public static bool Export(string filePath, string exportFormatId, Level level)
    {
        try
        {
            using AssimpContext context = new();
            Scene scene = new() { RootNode = new Node("RootNode") };
            
            Material defaultRoofMat = new() { Name = "DefaultRoofMaterial" };
            Material defaultWallMat = new() { Name = "DefaultWallMaterial" };
            Material defaultFloorMat = new() { Name = "DefaultFloorMaterial" };
            scene.Materials.Add(defaultRoofMat);
            scene.Materials.Add(defaultWallMat);
            scene.Materials.Add(defaultFloorMat);

            GenerateMesh(level, out Mesh roofMesh, out Mesh wallMesh, out Mesh floorMesh);
            roofMesh.MaterialIndex = 0;
            wallMesh.MaterialIndex = 0;
            floorMesh.MaterialIndex = 0;
                
            scene.Meshes.Add(roofMesh);
            scene.Meshes.Add(wallMesh);
            scene.Meshes.Add(floorMesh);
            scene.RootNode.MeshIndices.Add(0);
            scene.RootNode.MeshIndices.Add(1);
            scene.RootNode.MeshIndices.Add(2);
            
            Node meshNode = new("LevelMeshObject"); // meshes don't show up without an object in export
            meshNode.MeshIndices.Add(0);
            meshNode.MeshIndices.Add(1);
            meshNode.MeshIndices.Add(2);
            scene.RootNode.Children.Add(meshNode);

            return context.ExportFile(scene, filePath, exportFormatId);
        }
        catch (Exception e)
        {
            Debug.LogError($"{exportFormatId} export failed: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    private static void GenerateMesh(Level level, out Mesh roofMesh, out Mesh wallMesh, out Mesh floorMesh, string extraName = "")
    {
        roofMesh = new Mesh("LevelMeshRoof" + extraName, PrimitiveType.Triangle);
        wallMesh = new Mesh("LevelMeshWalls" + extraName, PrimitiveType.Triangle);
        floorMesh = new Mesh("LevelMeshFloor" + extraName, PrimitiveType.Triangle);

        GetLevelParts(level, out Line[] walls, out FlatPlane[] flatPlanes);
        CompileFlatPlanes(roofMesh, floorMesh, flatPlanes);
        CompileWalls(wallMesh, walls);
    }

    private static void GetLevelParts(Level level, out Line[] lines, out FlatPlane[] flatPlanesResult)
    {
        List<Line> walls = [];
        List<FlatPlane> flatPlanes = [];
        
        Dictionary<Point, List<Line>> pointMap = new();
        foreach (Line line in level.Lines) {
            Point[] points = line.GetPoints();
            if (!pointMap.ContainsKey(points[0])) pointMap[points[0]] = [];
            if (!pointMap.ContainsKey(points[1])) pointMap[points[1]] = [];
    
            pointMap[points[0]].Add(line);
            pointMap[points[1]].Add(line);
        }
        
        List<Line> visitedLines = [];
        foreach (Line startLine in level.Lines) {
            if (visitedLines.Contains(startLine)) continue;
            Point[] startpoints = startLine.GetPoints();

            List<Line> currentLoop = [];
            Line currentLine = startLine;
            Point currentPoint = startpoints[1];
    
            bool closed = false;
            while (true) {
                currentLoop.Add(currentLine);
                visitedLines.Add(currentLine);

                // Find the next line connected to currentPoint that isn't the one we just used
                Line? nextLine = pointMap[currentPoint]
                    .FirstOrDefault(l => l != currentLine && !visitedLines.Contains(l));
                Point[] nextpoints = nextLine?.GetPoints() ?? [];

                if (nextLine == null) {
                    if (currentPoint.Equals(startpoints[0])) closed = true;
                    break; 
                }

                // Move to the next point
                currentPoint = (nextpoints[0].Equals(currentPoint)) ? nextpoints[1] : nextpoints[0];
                currentLine = nextLine;
            }

            if (closed && currentLoop.Count >= 3) {
                flatPlanes.Add(new FlatPlane (
                    currentLoop.ToArray(),
                    new Vector3D(0, -1, 0)
                ));
                flatPlanes.Add(new FlatPlane (
                    currentLoop.ToArray(),
                    new Vector3D(0, 1, 0)
                ));
            }
        }

        foreach (FlatPlane plane in flatPlanes.Where(plane => plane.normal == new Vector3D(0, 1, 0))) 
            walls.AddRange(plane.points);
        
        lines = walls.ToArray();
        flatPlanesResult = flatPlanes.ToArray();
    }

    private static void CompileFlatPlanes(Mesh roofMesh, Mesh floorMesh, FlatPlane[] flatPlanesResult)
    {
        Vector3D[] GetOrderedPointLoop(FlatPlane plane, float height)
        {
            List<Vector3D> verts2D = [];
            Line firstLine = plane.points[0];
            Point[] pts = firstLine.GetPoints();

            Point current = pts[0];
            verts2D.Add(new Vector3D(current.position.x, height, current.position.y));

            foreach (Line line in plane.points)
            {
                Point[] linePts = line.GetPoints();
                Point nextPoint = linePts[0].Equals(current) ? linePts[1] : linePts[0];

                verts2D.Add(new Vector3D(nextPoint.position.x, height, nextPoint.position.y));
                current = nextPoint;
            }

            // Remove duplicate last point if loop closed
            if (verts2D.Count > 1 && verts2D[0] == verts2D[^1])
                verts2D.RemoveAt(verts2D.Count - 1);
            
            return verts2D.ToArray();
        }
        
        foreach (FlatPlane plane in flatPlanesResult)
        {
            if (plane.points.Length < 3)
                continue;
            
            Mesh mesh = plane.normal.Y < 0 ? floorMesh : roofMesh;

            float height = plane.normal.Y < 0 ? 0f : Line.wallHeight;
            Vector3D[] verts = GetOrderedPointLoop(plane, height);

            int baseIndex = mesh.VertexCount;
            // Add vertices
            foreach (Vector3D v in verts)
            {
                Vector3D scaled = v * ScaleFactor;
                mesh.Vertices.Add(scaled);
                mesh.Normals.Add(plane.normal);
            }

            // Add faces
            List<int> indices = TriangulatePolygon(verts, plane.normal); // Cannot resolve symbol 'TriangulatePolygon'
            foreach (List<int> tri in Chunk(indices, 3))
            {
                if (plane.normal.Y < 0) tri.Reverse(); // Reverse winding for floor vs roof
                mesh.Faces.Add(new Face([baseIndex + tri[0], baseIndex + tri[1], baseIndex + tri[2]]));
            }
        }
    }
    
    private static IEnumerable<List<int>> Chunk(List<int> source, int size) {
        for (int i = 0; i < source.Count; i += size)
            yield return source.GetRange(i, Math.Min(size, source.Count - i));
    }
    
    // TriangulatePolygon was generated using google: Gemini
    private static List<int> TriangulatePolygon(Vector3D[] vertices, Vector3D normal)
    {
        List<int> indices = new List<int>();
        List<int> V = new List<int>();
        for (int i = 0; i < vertices.Length; i++) V.Add(i);

        int n = V.Count;
        if (n < 3) return indices;

        // Helper to check if a point is inside a triangle
        bool PointInTriangle(Vector3D p, Vector3D a, Vector3D b, Vector3D c)
        {
            float det = (b.X - a.X) * (c.Z - a.Z) - (b.Z - a.Z) * (c.X - a.X);
            return det * ((a.X - p.X) * (b.Z - p.Z) - (a.Z - p.Z) * (b.X - p.X)) >= 0 &&
                   det * ((b.X - p.X) * (c.Z - p.Z) - (b.Z - p.Z) * (c.X - p.X)) >= 0 &&
                   det * ((c.X - p.X) * (a.Z - p.Z) - (c.Z - p.Z) * (a.X - p.X)) >= 0;
        }

        // Helper to calculate 2D signed area (to check for reflex vertices)
        float Area()
        {
            float area = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++)
                area += vertices[V[p]].X * vertices[V[q]].Z - vertices[V[q]].X * vertices[V[p]].Z;
            return area * 0.5f;
        }

        bool IsEar(int u, int v, int w, int n, int[] V)
        {
            Vector3D a = vertices[V[u]], b = vertices[V[v]], c = vertices[V[w]];
            // Check if triangle is convex (assuming counter-clockwise)
            if (((b.X - a.X) * (c.Z - a.Z) - (b.Z - a.Z) * (c.X - a.X)) <= 1e-10f) return false;
            // Check if any other vertex is inside this triangle
            for (int i = 0; i < n; i++)
            {
                if (i == u || i == v || i == w) continue;
                if (PointInTriangle(vertices[V[i]], a, b, c)) return false;
            }
            return true;
        }

        // Ensure we have CCW winding for the algorithm
        if (Area() < 0) V.Reverse();

        int count = 2 * n;
        for (int v = n - 1; n > 2; )
        {
            if (count-- <= 0) break; // Error: Likely a self-intersecting polygon

            int u = v; if (n <= u) u = 0;
            v = u + 1; if (n <= v) v = 0;
            int w = v + 1; if (n <= w) w = 0;

            if (IsEar(u, v, w, n, V.ToArray()))
            {
                indices.Add(V[u]);
                indices.Add(V[v]);
                indices.Add(V[w]);
                V.RemoveAt(v);
                n--;
                count = 2 * n;
            }
        }

        return indices;
    }
    
    private static void CompileWalls(Mesh mesh, Line[] walls)
    {
        void AddVertice(Vector3D givenPos, Vector3D normal)
        {
            Vector3D pos = givenPos * ScaleFactor;
            mesh.Vertices.Add(pos);
            mesh.Normals.Add(normal);
            mesh.Vertices.Add(pos); // one for the opposite direction
            mesh.Normals.Add(-normal);
        }
        
        // Since the walls are always perfectly straight, calculating this is rather simple
        Vector3D CalculateWallNormal(Vector3D start, Vector3D end)
        {
            // Get the direction of line
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;

            Vector3D normal = new(dy, 0, -dx);
            float length = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y);

            if (!(length > 0)) return normal;
            normal.X /= length;
            normal.Y /= length;

            return normal;
        }
        
        int i = mesh.VertexCount;
        foreach (Line line in walls)
        {
            Point[] points = line.GetPoints();
            if (points.Length != 2)
                continue;
            
            Vector3D startPos = new(points[0].position.x, 0, points[0].position.y);
            Vector3D endPos = new(points[1].position.x, 0, points[1].position.y);
            Vector3D normal = CalculateWallNormal(startPos, endPos);

            // 1. Add Vertices
            AddVertice(startPos, normal);                                                   // i+0, i+1 (Bottom Start)
            AddVertice(startPos + new Vector3D(0, Line.wallHeight, 0), normal);    // i+2, i+3 (Top Start)
            AddVertice(endPos, normal);                                                     // i+4, i+5 (Bottom End)
            AddVertice(endPos + new Vector3D(0, Line.wallHeight, 0), normal);      // i+6, i+7 (Top End)

            // 2. Front Faces
            mesh.Faces.Add(new Face([i + 0, i + 2, i + 6]));    // Triangle 1: BottomStart, TopStart, TopEnd
            mesh.Faces.Add(new Face([i + 0, i + 6, i + 4]));    // Triangle 2: BottomStart, TopEnd, BottomEnd

            // 3. Back Faces
            mesh.Faces.Add(new Face([i + 1, i + 7, i + 3]));    // Triangle 1: BottomStart, TopEnd, TopStart
            mesh.Faces.Add(new Face([i + 1, i + 5, i + 7]));    // Triangle 2: BottomStart, BottomEnd, TopEnd

            i += 8;
        }
    }
}