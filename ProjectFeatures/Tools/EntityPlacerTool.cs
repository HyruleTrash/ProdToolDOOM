using Microsoft.Xna.Framework.Graphics;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public class EntityPlacerTool : BasePlacerTool
{
    private readonly Texture2D entityTexture;
    
    public EntityPlacerTool(WindowInstance windowRef) : base(windowRef)
    {
        this.entityTexture = Program.instance.Content.Load<Texture2D>("Icons/Entity");
        this.toCall = () => Program.instance.cmdHistory.ApplyCmd(new AddEntityCmd(Project.instance, this.lastMousePosition, this.entityTexture, windowRef));
    }

    public override void SetVisuals()
    {
        Debug.Log("EntityPlacerTool::SetVisuals");
    }
}