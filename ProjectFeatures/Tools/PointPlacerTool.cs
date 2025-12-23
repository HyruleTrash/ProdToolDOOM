using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public class PointPlacerTool : BasePlacerTool
{
    private readonly Texture2D pointTexture;

    public PointPlacerTool()
    {
        pointTexture = Program.instance.Content.Load<Texture2D>("Icons/Point");
        toCall = () => Program.instance.cmdHistory.ApplyCmd(new AddPointCmd(Project.instance, lastMousePosition, pointTexture));

        rightClickManager.instance.AddOptions<Point>([
            new rightClickManager.RightClickOption(
                "Remove", 
                () => Program.instance.cmdHistory.ApplyCmd(new RemovePointCmd(Project.instance))
            )
        ]);
    }

    public override void SetVisuals()
    {
        Debug.Log("PointPlacerTool::SetVisuals");
    }
}