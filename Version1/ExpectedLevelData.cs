using System.Xml;

namespace ProdToolDOOM.Version1;

public class ExpectedLevelData : ExpectedData, IExpectedCollectionData
{
    public Level level = new ();
    private readonly WindowsProjectLoadStrategy referenceLoadStrategy;

    public ExpectedLevelData(WindowsProjectLoadStrategy referenceLoadStrategy)
    {
        this.referenceLoadStrategy = referenceLoadStrategy;
        name = "Levels";
    }

    private class ExpectedEntitiesData : ExpectedData, IExpectedCollectionData
    {
        private Entity? entity = null;
        private ExpectedLevelData referenceLevelData;

        public ExpectedEntitiesData(ExpectedLevelData referenceLevelData)
        {
            name = "Entities";
            this.referenceLevelData = referenceLevelData;
        }
        
        public void loadEntry(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Element)
                return;
            if (reader.Name == "Entity")
            {
                if (entity != null)
                    referenceLevelData.level.Entities.Add(entity);
            }
                
            if (entity == null)
                entity = new Entity();

            if (reader.Name == "Id")
                entity.Id = reader.ReadElementContentAsInt();
            else if (reader.Name == "XPosition")
                entity.XPosition = reader.ReadElementContentAsFloat();
            else if (reader.Name == "YPosition")
                entity.YPosition = reader.ReadElementContentAsFloat();
        }

        public void saveEntry() {}
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