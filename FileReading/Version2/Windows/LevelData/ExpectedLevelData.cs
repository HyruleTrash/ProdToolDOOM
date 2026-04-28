
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
        this.name = "Levels";
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return;

        if (reader.Name == "Level")
        {
            this.level ??= new Level();
            reader.Read();
        }
        
        if (reader.Name == "IdCounter") this.level.levelObjectIdCounter = reader.ReadElementContentAsInt();

        this.referenceLoadStrategy.ReadData(reader, [
            new ExpectedEntitiesData(this) { stopAt = "Entities" },
            new ExpectedPointsData(this) { stopAt = "Points" },
            new ExpectedLinesData(this)  { stopAt = "Lines" }
        ]);
        
        Debug.Log($"{reader.Name}, {reader.NodeType} aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
    }

    public void saveEntry()
    {
        if (this.level == null) return;
        Debug.Log("Saving Level Data");
        Project.instance.levels.Add(this.level);
        foreach (Line line in this.level.Lines) line.Init();
        this.level = null;
    }
}
#endif