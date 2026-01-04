namespace ProdToolDOOM.ProjectFeatures;

public class RightClickRegister
{
    public RightClickRegister(rightClickManager manager)
    {
        manager.AddOptions<Point>([
            new rightClickManager.RightClickOption(
                "Remove", 
                () =>
                {
                    if (manager.currentVisual is null)
                        return;
                    if (manager.currentVisual?.currentSelection is Point pt)
                        Program.instance.cmdHistory.ApplyCmd(new RemovePointCmd(Project.instance, pt, manager.Reset));
                })
        ]);
    }
}