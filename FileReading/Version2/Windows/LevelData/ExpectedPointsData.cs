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
        this.name = "Points";
        this.referenceLevelData = referenceLevelData;
    }
    
    public void loadEntry(XmlReader reader)
    {
        if (this.point == null)
        {
            Texture2D? pointTexture = Program.instance.Content.Load<Texture2D>("Icons/Point");
            this.point = new Point(Vector2.Zero, pointTexture, -1, -1, Program.instance,
                Project.instance);
        }
        if (reader.NodeType != XmlNodeType.Element) return;

        if (reader.Name == "LevelId") this.point.LevelId = reader.ReadElementContentAsInt();
        if (reader.Name == "LevelObjectId") this.point.LevelObjectId = reader.ReadElementContentAsInt();
        if (reader.Name == "Position") this.point.Position = Vector2.FromString(reader.ReadElementContentAsString());
    }

    public void saveEntry()
    {
        if (this.point == null) return;
        Debug.Log($"Saving point: {this.point}");
        this.referenceLevelData.level.Add(this.point);
        this.point.UpdateVisualPosition(Program.instance.GetWindowSize());
        this.point = null;
    }
}
#endif