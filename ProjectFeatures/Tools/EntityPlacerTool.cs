using Microsoft.Xna.Framework.Graphics;

namespace DLLevelBuilder.ProjectFeatures.Tools;

public class EntityPlacerTool : BasePlacerTool
{
    private readonly Texture2D entityTexture;
    
    public EntityPlacerTool(WindowInstance windowRef) : base(windowRef)
    {
        this.entityTexture = Program.instance.Content.Load<Texture2D>("Icons/Entity");
        this.toCall = () => Program.instance.cmdHistory.ApplyCmd(new AddEntityCmd(Project.Instance, this.lastMousePosition, this.entityTexture, windowRef));
    }

    public override void SetVisuals()
    {
        Debug.Log("EntityPlacerTool::SetVisuals");
    }
}