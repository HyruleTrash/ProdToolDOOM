
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
        Debug.Log($"tempest log: {reader.NodeType}, {reader.Name}");
        if (reader.NodeType != XmlNodeType.Element)
            return;

        if (reader.Name == "Level")
        {
            this.level ??= new Level();
            reader.Read();
        }
        
        if (reader.Name == "IdCounter")
            level.levelObjectIdCounter = reader.ReadElementContentAsInt();
        
        Debug.Log($"temp log: {level.levelObjectIdCounter}, {reader.Name}");
        
        referenceLoadStrategy.ReadData(reader, [
            new ExpectedEntitiesData(this) { stopAt = "Entities" },
            new ExpectedPointsData(this) { stopAt = "Points" },
            new ExpectedLinesData(this)  { stopAt = "Lines" }
        ]);
    }

    public void saveEntry()
    {
        if (level == null) return;
        Debug.Log("Saving Level Data");
        Project.instance.levels.Add(level);
        level = null;
    }
}
#endif