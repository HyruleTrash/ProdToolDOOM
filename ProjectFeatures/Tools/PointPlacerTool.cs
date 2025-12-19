using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public class PointPlacerTool : ITool
{
    private bool ableToPlace = true;
    
    public void Call(MouseState mouse)
    {
        if (!ableToPlace) return;
        
        // TODO check if mouse pos was in canvas bounds
        var position = new Vector2(mouse.X, mouse.Y);
        Program.instance.cmdHistory.ApplyCmd(new AddPointCmd(Project.instance, position));
        
        ableToPlace = false;
    }

    public void UnEquip()
    {
        ableToPlace = true;
    }

    public void Update(float dt, MouseState mouse)
    {
        if (!ableToPlace && mouse.LeftButton == ButtonState.Released)
            ableToPlace = true;
    }

    public void SetVisuals()
    {
        Debug.Log("PointPlacerTool::SetVisuals");
        // throw new NotImplementedException();
    }
}