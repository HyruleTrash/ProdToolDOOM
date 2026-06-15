using DLLevelBuilder.ProjectFeatures.Tools;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace DLLevelBuilder.ProjectFeatures;

public static class BaseShortcuts
{
    public static readonly ShortcutManager.ShortCut[] Shortcuts =
    [
        new([Keys.LeftControl, Keys.Z], Program.instance.cmdHistory.UndoCmd),
        new([Keys.LeftControl, Keys.LeftShift, Keys.Z], Program.instance.cmdHistory.RedoCmd),
        new([Keys.LeftControl, Keys.LeftShift, Keys.S], () => Program.instance.cmdHistory.ApplyCmd(new SaveProjectAsNewCmd(Project.Instance))),
        new([Keys.LeftControl, Keys.S], () => Program.instance.cmdHistory.ApplyCmd(new SaveProjectCmd(Project.Instance))),
        new([Keys.LeftControl, Keys.O], () => Program.instance.cmdHistory.ApplyCmd(new LoadProjectCmd(Project.Instance))),
        new([Keys.LeftControl, Keys.N], () => Program.instance.cmdHistory.ApplyCmd(new AddLevelCmd(Project.Instance))),
        new([Keys.LeftControl, Keys.M], EntityCreationPopup.ToggleVisibility),
        new([Keys.LeftControl, Keys.Left], () => Program.instance.cmdHistory.ApplyCmd(new SwitchLevelCmd(Project.Instance, -1))),
        new([Keys.LeftControl, Keys.Right], () => Program.instance.cmdHistory.ApplyCmd(new SwitchLevelCmd(Project.Instance, 1))),
        new([Keys.LeftControl, Keys.Q], () => ToolManager.SetTool(typeof(PointPlacerTool))),
        new([Keys.LeftControl, Keys.W], () => ToolManager.SetTool(typeof(EntityPlacerTool))),
    ];
}