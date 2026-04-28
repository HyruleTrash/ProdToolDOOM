namespace ProdToolDOOM.ProjectFeatures.Tools;

public class EntityPlacerTool : BasePlacerTool
{
    public EntityPlacerTool(WindowInstance windowRef) : base(windowRef)
    {
        this.toCall = () => Program.instance.cmdHistory.ApplyCmd(new AddEntityCmd(Project.instance, this.lastMousePosition));
    }

    public override void SetVisuals()
    {
        Debug.Log("EntityPlacerTool::SetVisuals");
    }
}