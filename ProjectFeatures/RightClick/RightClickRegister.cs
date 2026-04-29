namespace ProdToolDOOM.ProjectFeatures;

public static class RightClickRegister
{
    public static void Register(RightClickManager manager)
    {
        manager.AddOptions<Point>([
            new RightClickManager.RightClickOption(
                "Remove", 
                () =>
                {
                    if (manager.currentVisual is null)
                        return;
                    if (manager.currentVisual?.currentSelection is Point pt)
                        Program.instance.cmdHistory.ApplyCmd(new RemovePointCmd(Project.instance, pt, manager.Reset));
                })
        ]);
        manager.AddOptions<Line>([
            new RightClickManager.RightClickOption(
                "Remove", 
                () =>
                {
                    if (manager.currentVisual is null)
                        return;
                    if (manager.currentVisual?.currentSelection is Line ln)
                        Program.instance.cmdHistory.ApplyCmd(new RemoveLineCmd(Project.instance, ln, manager.Reset));
                })
        ]);
    }
}