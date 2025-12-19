
#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version1;

public class ExpectedLevelData : ExpectedData, IExpectedCollectionData
{
    public Level level = new ();
    public List<Vector2> points = [];
    public List<Line> lines = [];
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
        points = [];
        lines = [];
        
        referenceLoadStrategy.ReadData(reader, [
            new ExpectedEntitiesData(this) { stopAt = "Entities" },
            new ExpectedPointsData(this) { stopAt = "Points" },
            new ExpectedLinesData(this)  { stopAt = "Lines" }
        ]);
        
        level.SetLevelGeometry(points, lines);
    }

    public void saveEntry()
    {
        Project.instance.levels.Add(level);
    }
}
#endif