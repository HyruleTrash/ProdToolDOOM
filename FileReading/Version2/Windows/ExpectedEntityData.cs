
#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version2;

public class ExpectedEntityData : ExpectedData, IExpectedCollectionData
{
    private readonly ProjectLoadStrategy referenceLoadStrategy;
    private int lastReadId = -1;
    private EntityData entityData = new();

    public ExpectedEntityData(ProjectLoadStrategy referenceLoadStrategy)
    {
        this.referenceLoadStrategy = referenceLoadStrategy;
        this.name = "EntityData";
    }
        
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element) return;
        
        if (reader.Name == nameof(Int32)) this.lastReadId = reader.ReadElementContentAsInt();
        if (reader.Name == "ImagePath") this.entityData.ImagePath = reader.ReadElementContentAsString();
        if (reader.Name == "Name") this.entityData.Name = reader.ReadElementContentAsString();
    }

    public void saveEntry() => Project.instance.entityDatas.Add(this.lastReadId, new(this.entityData));
}
#endif