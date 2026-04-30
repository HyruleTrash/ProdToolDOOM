
#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version1;

public class ExpectedLinesData : ExpectedData, IExpectedCollectionData
{
    private Line? line;
    private readonly ExpectedLevelData referenceLevelData;

    public ExpectedLinesData(ExpectedLevelData referenceLevelData)
    {
        this.name = "Lines";
        this.referenceLevelData = referenceLevelData;
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return;

        this.line ??= new Line(Project.instance);

        if (reader.Name == "Id")
            this.line.Id = reader.ReadElementContentAsInt();
        else if (reader.Name == "IdOther") this.line.IdOther = reader.ReadElementContentAsInt();
    }

    public void saveEntry()
    {
        if (this.line == null)
            return;
        Debug.Log($"Saving line: {this.line}");
        Project projectRef = Project.instance;
        this.line.levelId = projectRef.levels.Count;
        this.referenceLevelData.level.Add(new Line(projectRef, this.line));
    }
}
#endif