
#if WINDOWS
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace ProdToolDOOM.Version1;

public class ExpectedLevelData : ExpectedData, IExpectedCollectionData
{
    public Level level = new ();
    private readonly ProjectLoadStrategy referenceLoadStrategy;
    private Texture2D? pointTexture;

    public ExpectedLevelData(ProjectLoadStrategy referenceLoadStrategy)
    {
        this.referenceLoadStrategy = referenceLoadStrategy;
        this.name = "Levels";
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return;
        this.level = new Level();

        this.referenceLoadStrategy.ReadData(reader, [
            new ExpectedEntitiesData(this) { stopAt = "Entities" },
            new ExpectedPointsData(this) { stopAt = "Points" },
            new ExpectedLinesData(this)  { stopAt = "Lines" }
        ]);
    }

    public void saveEntry()
    {
        Project.instance.levels.Add(this.level);
    }
}
#endif