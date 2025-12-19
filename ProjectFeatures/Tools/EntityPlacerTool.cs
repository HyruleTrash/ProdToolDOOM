namespace ProdToolDOOM.ProjectFeatures.Tools;

public class EntityPlacerTool : BasePlacerTool
{
    public EntityPlacerTool()
    {
        toCall = () => Program.instance.cmdHistory.ApplyCmd(new AddEntityCmd(Project.instance, lastMousePosition));
    }

    public override void SetVisuals()
    {
        Debug.Log("EntityPlacerTool::SetVisuals");
    }
}