using Microsoft.Xna.Framework.Input;

namespace ProdToolDOOM.ProjectFeatures.Tools;

public interface ITool : IBaseUpdatable
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
    /// Sets any needed visuals, such as tool selected icon
    /// </summary>
    public void SetVisuals();
}