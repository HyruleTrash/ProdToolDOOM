using Microsoft.Xna.Framework.Input;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public interface ITool
{
    /// <summary>
    /// Triggers the tool to do its main function
    /// </summary>
    public void Call(MouseState mouse);
    
    /// <summary>
    /// Extra function for if the tool needs resetting upon unequip
    /// </summary>
    public virtual void UnEquip() {}
    
    /// <summary>
    /// Updates any live data of the tool
    /// </summary>
    /// <param name="dt">delta time</param>
    /// <param name="mouse">current state of the mouse</param>
    public void Update(float dt, MouseState mouse);
    
    /// <summary>
    /// Sets any needed visuals, such as tool selected icon
    /// </summary>
    public void SetVisuals();
}