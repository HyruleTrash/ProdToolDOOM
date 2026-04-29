#if WINDOWS
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace ProdToolDOOM.Version2;

public class ExpectedEntitiesData : ExpectedData, IExpectedCollectionData
{
    private Entity? entity;
    private readonly ExpectedLevelData referenceLevelData;

    public ExpectedEntitiesData(ExpectedLevelData referenceLevelData)
    {
        this.name = "Entities";
        this.referenceLevelData = referenceLevelData;
    }
        
    public void loadEntry(XmlReader reader)
    {
        if (this.entity == null)
        {
            Texture2D? entityTexture = Program.instance.Content.Load<Texture2D>("Icons/Entity");
            this.entity = new Entity(-1, entityTexture, Program.instance, Project.instance, -1, Vector2.Zero);
        }
        if (reader.NodeType != XmlNodeType.Element) return;

        if (reader.Name == "LevelId") this.entity.LevelId = reader.ReadElementContentAsInt();
        if (reader.Name == "LevelObjectId") this.entity.LevelObjectId = reader.ReadElementContentAsInt();
        if (reader.Name == "Position") this.entity.Position = Vector2.FromString(reader.ReadElementContentAsString());
        if (reader.Name == "DataId") this.entity.DataId = reader.ReadElementContentAsInt();
    }

    public void saveEntry()
    {
        if (this.entity == null) return;
        Debug.Log($"Saving entity: {this.entity}");
        this.referenceLevelData.level.Add(this.entity);
        Project.instance.entityDatas[this.entity.DataId].AddEntityRegistration(this.entity);
        this.entity.UpdateVisualPosition(Program.instance.GetWindowSize());
        this.entity = null;
    }
}
#endif