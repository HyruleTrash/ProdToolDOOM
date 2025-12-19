#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version2;

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
        var setEntity = new Entity(entity);
        referenceLevelData.level.Entities.Add(setEntity);
        try
        {
            Project.Instance.entityDatas[setEntity.Id].AddEntityRegistration(setEntity);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
#endif