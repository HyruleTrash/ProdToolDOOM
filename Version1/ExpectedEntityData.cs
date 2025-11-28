using System.Xml;

namespace ProdToolDOOM.Version1;

public class ExpectedEntityData : ExpectedData, IExpectedCollectionData
{
    private readonly WindowsProjectLoadStrategy referenceLoadStrategy;
    private int lastReadId = -1;
    private EntityData entityData = new();

    public ExpectedEntityData(WindowsProjectLoadStrategy referenceLoadStrategy)
    {
        this.referenceLoadStrategy = referenceLoadStrategy;
        name = "EntityData";
    }
        
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element) return;
        
        if (reader.Name == nameof(Int32))
            lastReadId = reader.ReadElementContentAsInt();
        if (reader.Name == "ImagePath")
            entityData.ImagePath = reader.ReadElementContentAsString();
        if (reader.Name == "Name")
            entityData.Name = reader.ReadElementContentAsString();
    }

    public void saveEntry()
    {
        Project.entityDatas.Add(lastReadId, new(entityData));
    }
}