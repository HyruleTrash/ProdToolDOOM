namespace DLLevelBuilder.ProjectFeatures;

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
                        Program.instance.cmdHistory.ApplyCmd(new RemovePointCmd(Project.Instance, pt, manager.Reset));
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
                        Program.instance.cmdHistory.ApplyCmd(new RemoveLineCmd(Project.Instance, ln, manager.Reset));
                })
        ]);
        manager.AddOptions<Entity>([
            new RightClickManager.RightClickOption(
                "Remove", 
                () =>
                {
                    if (manager.currentVisual is null)
                        return;
                    if (manager.currentVisual?.currentSelection is Entity e)
                        Program.instance.cmdHistory.ApplyCmd(new RemoveEntityCmd(Project.Instance, e, manager.Reset));
                }),
            new RightClickManager.RightClickOption(
                "Set name", 
                () =>
                {
                    if (manager.currentVisual is null)
                        return;
                    if (manager.currentVisual?.currentSelection is Entity e)
                    {
                        EntitySetIdPopup.ToggleVisibility();
                        EntitySetIdPopup.Instance.SetCurrentSelection(e);
                    }
                })
        ]);
    }
}