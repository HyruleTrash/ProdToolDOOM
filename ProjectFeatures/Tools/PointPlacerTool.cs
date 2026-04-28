using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public class PointPlacerTool : BasePlacerTool
{
    private readonly Texture2D pointTexture;

    public PointPlacerTool(WindowInstance windowRef) : base(windowRef)
    {
        this.pointTexture = Program.instance.Content.Load<Texture2D>("Icons/Point");
        this.toCall = () => Program.instance.cmdHistory.ApplyCmd(new AddPointCmd(Project.instance, this.lastMousePosition, this.pointTexture, windowRef));
    }

    public override void SetVisuals()
    {
        Debug.Log("PointPlacerTool::SetVisuals");
    }
}