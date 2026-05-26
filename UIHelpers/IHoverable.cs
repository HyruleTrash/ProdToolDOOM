using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DLLevelBuilder;

public interface IHoverable
{
    public bool CheckHover(MouseState mouseState, float dt);
}