
#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version2;

public class ExpectedLevelData : ExpectedData, IExpectedCollectionData
{
    public Level level = new ();
    private readonly ProjectLoadStrategy referenceLoadStrategy;

    public ExpectedLevelData(ProjectLoadStrategy referenceLoadStrategy)
    {
        this.referenceLoadStrategy = referenceLoadStrategy;
        name = "Levels";
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return;
        level = new Level();
        
        if (reader.Name == "IdCounter")
            level.levelObjectIdCounter = reader.ReadElementContentAsInt();
        
        referenceLoadStrategy.ReadData(reader, [
            new ExpectedEntitiesData(this) { stopAt = "Entities" },
            new ExpectedPointsData(this) { stopAt = "Points" },
            new ExpectedLinesData(this)  { stopAt = "Lines" }
        ]);
    }

    public void saveEntry()
    {
        Project.instance.levels.Add(level);
    }
}
#endif