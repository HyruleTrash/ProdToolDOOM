using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ProdToolDOOM;

public static class BaseShortcuts
{
    public static ShortcutManager.ShortCut[] baseShortcuts =
    [
        new([Keys.LeftControl, Keys.Z], Program.instance.cmdHistory.UndoCmd),
        new([Keys.LeftControl, Keys.LeftShift, Keys.Z], Program.instance.cmdHistory.RedoCmd),
    ];
}