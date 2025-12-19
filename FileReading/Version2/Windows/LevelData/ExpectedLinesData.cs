
#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version2;

public class ExpectedLinesData : ExpectedData, IExpectedCollectionData
{
    private Line line = new();
    private readonly ExpectedLevelData referenceLevelData;

    public ExpectedLinesData(ExpectedLevelData referenceLevelData)
    {
        name = "Lines";
        this.referenceLevelData = referenceLevelData;
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return;

        if (reader.Name == "Id")
            line.Id = reader.ReadElementContentAsInt();
        else if (reader.Name == "IdOther")
            line.IdOther = reader.ReadElementContentAsInt();
    }

    public void saveEntry()
    {
        referenceLevelData.lines.Add(new Line(line));
    }
}
#endif