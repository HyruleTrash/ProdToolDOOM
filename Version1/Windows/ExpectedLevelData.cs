using System.Xml;

#if WINDOWS
namespace ProdToolDOOM.Version1;

public class ExpectedLevelData : ExpectedData, IExpectedCollectionData
{
    private Level level = new ();
    private readonly ProjectLoadStrategy referenceLoadStrategy;

    public ExpectedLevelData(ProjectLoadStrategy referenceLoadStrategy)
    {
        this.referenceLoadStrategy = referenceLoadStrategy;
        name = "Levels";
    }

    private class ExpectedEntitiesData : ExpectedData, IExpectedCollectionData
    {
        private Entity entity = new();
        private readonly ExpectedLevelData referenceLevelData;

        public ExpectedEntitiesData(ExpectedLevelData referenceLevelData)
        {
            name = "Entities";
            this.referenceLevelData = referenceLevelData;
        }
        
        public void loadEntry(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Element)
                return;

            if (reader.Name == "Id")
                entity.Id = reader.ReadElementContentAsInt();
            else if (reader.Name == "XPosition")
                entity.XPosition = reader.ReadElementContentAsFloat();
            else if (reader.Name == "YPosition")
                entity.YPosition = reader.ReadElementContentAsFloat();
        }

        public void saveEntry()
        {
            referenceLevelData.level.Entities.Add(new Entity(entity));
        }
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return;
        level = new Level();
        
        referenceLoadStrategy.ReadData(reader, [
            new ExpectedEntitiesData(this)
        ], "Level");

        Project.levels.Add(level);
    }

    public void saveEntry() { }
}
#endif