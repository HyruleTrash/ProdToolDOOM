
#if WINDOWS
using System.Xml;

namespace DLLevelBuilder.Version3;

public class ExpectedLevelData : ExpectedData, IExpectedCollectionData
{
    public Level level = new();
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

        this.level ??= new Level();

        this.referenceLoadStrategy.ReadData(reader, [
            new ExpectedData { name = "IdCounter", stopAt = "IdCounter", 
                load = rdr => this.level.levelObjectIdCounter = rdr.ReadElementContentAsInt()
            },
            new ExpectedData { name = "LevelId", stopAt = "LevelId", 
                load = rdr => this.level.LevelId = rdr.ReadElementContentAsInt()
            },
            new ExpectedEntitiesData(this) { stopAt = "Entities" },
            new ExpectedPointsData(this) { stopAt = "Points" },
            new ExpectedLinesData(this)  { stopAt = "Lines" }
        ]);
    }

    public void saveEntry()
    {
        if (this.level == null) return;
        Debug.Log("Saving Level Data");
        Project.Instance.levels.Add(this.level.LevelId, this.level);
        this.level = null;
    }
}
#endif