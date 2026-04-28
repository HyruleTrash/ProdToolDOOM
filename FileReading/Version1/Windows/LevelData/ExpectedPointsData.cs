#if WINDOWS
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace ProdToolDOOM.Version1;

public class ExpectedPointsData : ExpectedData, IExpectedCollectionData
{
    private Vector2 vector2 = new();
    private readonly ExpectedLevelData referenceLevelData;
    private Texture2D? pointTexture;

    public ExpectedPointsData(ExpectedLevelData referenceLevelData)
    {
        this.name = "Points";
        this.referenceLevelData = referenceLevelData;
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return;

        if (reader.Name == "Vector2")
        {
            this.vector2 = Vector2.FromString(reader.ReadElementContentAsString());
            Debug.Log($"Registering point: {this.vector2}");
        }
    }

    public void saveEntry()
    {
        Debug.Log($"Saving point: {this.vector2}");
        this.pointTexture ??= Program.instance.Content.Load<Texture2D>("Icons/Point");
        Project projectRef = Project.instance;
        int levelId = projectRef.levels.Count;
        this.referenceLevelData.level.Add(new Point(this.vector2, this.pointTexture, projectRef.levels[levelId].levelObjectIdCounter++, levelId, Program.instance, projectRef));
        this.vector2 = new Vector2();
    }
}
#endif