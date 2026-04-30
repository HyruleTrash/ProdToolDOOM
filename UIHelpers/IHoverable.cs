using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProdToolDOOM;

public interface IHoverable
{
    public bool CheckHover(MouseState mouseState, float dt);
}