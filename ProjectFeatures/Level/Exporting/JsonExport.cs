using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DLLevelBuilder.ProjectFeatures.Exporting;

public class JsonExport : ExportOption
{
    public JsonExport() : base("Json files", ".json") { }

    private struct Vec2
    {
        public Vec2(Vector2 pos)
        {
            this.x = pos.x;
            this.y = pos.y;
        }

        public float x { get; set; }
        public float y { get; set; }
    }
    
    private class EntityData
    {
        public string name { get; set; }
        public int id { get; set; }
    }
    
    private class Entity
    {
        public Vec2 position { get; set; }
        public int id { get; set; }
        public int referenceId { get; set; }
    }
    
    private class Point
    {
        public Vec2 position { get; set; }
        public int id { get; set; }
    }
    
    private class Line
    {
        public int pointId { get; set; }
        public int pointIdOther { get; set; }
    }
    
    private class Level
    {
        public EntityData[] entityDatas { get; set; }
        public Entity[] entities { get; set; }
        public Point[] points { get; set; }
        public Line[] lines { get; set; }
    }
    
    public override bool Export(string valueFilePath, DLLevelBuilder.Level level)
    {
        try
        {
            Level levelToSerialize = new();

            List<EntityData> entityDatas = [];
            entityDatas.AddRange(Project.Instance.EntityDatas.Select(data => new EntityData { id = data.Key, name = data.Value.Name }));
            levelToSerialize.entityDatas = entityDatas.ToArray();
        
            List<Entity> entities = [];
            entities.AddRange(level.Entities.Select(data => new Entity { id = data.LevelObjectId, referenceId = data.DataId, position = new Vec2(data.position)}));
            levelToSerialize.entities = entities.ToArray();

            List<Point> points = [];
            points.AddRange(level.Points.Select(data => new Point { id = data.LevelObjectId, position = new Vec2(data.position)}));
            levelToSerialize.points = points.ToArray();

            List<Line> lines = [];
            lines.AddRange(level.Lines.Select(data => new Line { pointId = data.Id, pointIdOther = data.IdOther}));
            levelToSerialize.lines = lines.ToArray();
        
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
            };

            string json = JsonSerializer.Serialize(levelToSerialize, options);

            File.WriteAllText(valueFilePath, json);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Export to json failed: {e.Message} {e.StackTrace}");
            return false;
        }
    }
}