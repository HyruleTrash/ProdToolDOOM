#if WINDOWS
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace ProdToolDOOM.Version2;

public class ExpectedPointsData : ExpectedData, IExpectedCollectionData
{
    private Vector2 vector2 = new();
    private readonly ExpectedLevelData referenceLevelData;
    private Texture2D? pointTexture;

    public ExpectedPointsData(ExpectedLevelData referenceLevelData)
    {
        name = "Points";
        this.referenceLevelData = referenceLevelData;
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            return;

        if (reader.Name == "Vector2")
        {
            vector2 = Vector2.FromString(reader.ReadElementContentAsString());
            Debug.Log($"Registering point: {vector2}");
        }
    }

    public void saveEntry()
    {
        Debug.Log($"Saving point: {vector2}");
        pointTexture ??= Program.instance.Content.Load<Texture2D>("Icons/Point");
        referenceLevelData.level.Add(new Point(vector2, pointTexture, Project.instance.levels.Count));
        vector2 = new Vector2();
    }
}
#endif