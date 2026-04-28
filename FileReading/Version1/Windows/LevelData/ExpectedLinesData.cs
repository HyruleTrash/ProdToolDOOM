
#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version1;

public class ExpectedLinesData : ExpectedData, IExpectedCollectionData
{
    private Line? line;
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

        line ??= new(Project.instance);

        if (reader.Name == "Id")
            line.Id = reader.ReadElementContentAsInt();
        else if (reader.Name == "IdOther")
            line.IdOther = reader.ReadElementContentAsInt();
    }

    public void saveEntry()
    {
        if (line == null)
            return;
        Debug.Log($"Saving line: {line}");
        var projectRef = Project.instance;
        line.levelId = projectRef.levels.Count;
        referenceLevelData.level.Add(new Line(projectRef, line));
    }
}
#endif