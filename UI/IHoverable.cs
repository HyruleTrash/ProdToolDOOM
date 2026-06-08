using Microsoft.Xna.Framework.Input;

namespace DLLevelBuilder.UI;

public interface IHoverable
{
    public bool CheckHover(MouseState mouseState, float dt);
}