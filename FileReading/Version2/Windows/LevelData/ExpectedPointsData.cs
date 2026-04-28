#if WINDOWS
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace ProdToolDOOM.Version2;

public class ExpectedPointsData : ExpectedData, IExpectedCollectionData
{
    private Point? point;
    private readonly ExpectedLevelData referenceLevelData;

    public ExpectedPointsData(ExpectedLevelData referenceLevelData)
    {
        name = "Points";
        this.referenceLevelData = referenceLevelData;
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (point == null)
        {
            var pointTexture = Program.instance.Content.Load<Texture2D>("Icons/Point");
            point = new Point(Vector2.Zero, pointTexture, -1, -1, Program.instance,
                Project.instance);
        }
        // Debug.Log($"{reader.Name} {reader.NodeType}");
        if (reader.NodeType != XmlNodeType.Element)
            return;

        if (reader.Name == "LevelId")
            point.LevelId = reader.ReadElementContentAsInt();
        else if (reader.Name == "LevelObjectId")
            point.LevelObjectId = reader.ReadElementContentAsInt();
        else if (reader.Name == "Position")
            point.Position = Vector2.FromString(reader.ReadElementContentAsString());
    }

    public void saveEntry()
    {
        if (point == null) return;
        Debug.Log($"Saving point: {point}");
        referenceLevelData.level.Add(point);
        point.UpdateVisualPosition(Program.instance.GetWindowSize());
        point = null;
    }
}
#endif