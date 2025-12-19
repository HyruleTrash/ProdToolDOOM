#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version1;

public class ExpectedEntitiesData : ExpectedData, IExpectedCollectionData
{
    private Entity entity = new();
    private readonly ExpectedLevelData referenceLevelData;

    public ExpectedEntitiesData(ExpectedLevelData referenceLevelData)
    {
        name = "Entities";
        this.referenceLevelData = referenceLevelData;
    }
        
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return;

        if (reader.Name == "Id")
            entity.Id = reader.ReadElementContentAsInt();
        else if (reader.Name == "Position")
            entity.Position = Vector2.FromString(reader.ReadElementContentAsString());
    }

    public void saveEntry()
    {
        referenceLevelData.level.Add(new Entity(entity));
    }
}
#endif