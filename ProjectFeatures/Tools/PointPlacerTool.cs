using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public class PointPlacerTool : BasePlacerTool
{
    public PointPlacerTool()
    {
        toCall = () => Program.instance.cmdHistory.ApplyCmd(new AddPointCmd(Project.instance, lastMousePosition));
    }

    public override void SetVisuals()
    {
        Debug.Log("PointPlacerTool::SetVisuals");
    }
}