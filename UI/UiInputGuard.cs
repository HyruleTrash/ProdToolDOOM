using Microsoft.Xna.Framework;

namespace DLLevelBuilder.UI;

public static class UiInputGuard
{
    private static TimeSpan lockoutEndTime = TimeSpan.Zero;
    public static void Lock(TimeSpan duration, GameTime gameTime) => lockoutEndTime = gameTime.TotalGameTime + duration;
    public static bool IsLocked(GameTime gameTime) => gameTime.TotalGameTime < lockoutEndTime;
}