using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;

namespace DLLevelBuilder;

public static class UIHelper
{
    /// <summary>
    /// Adds created UI, to a given parent
    /// </summary>
    /// <param name="parent">GraphicalUiElement or FrameworkElement that will hold your addition</param>
    /// <param name="child">represents the element you want to add</param>
    private static void BaseAddUI(object parent, object child)
    {
        GraphicalUiElement? childVisual = child switch
        {
            FrameworkElement fe => fe.Visual,
            GraphicalUiElement gue => gue,
            _ => null
        };
        GraphicalUiElement? parentVisual = parent switch
        {
            FrameworkElement fe => fe.Visual,
            GraphicalUiElement gue => gue,
            _ => null
        };
        
        if (childVisual == null) return;
        
        if (parentVisual == null)
            childVisual.AddToRoot();
        else
            parentVisual.AddChild(childVisual);
    }
    
    /// <summary>
    /// Adds created UI, to a given parent
    /// </summary>
    /// <param name="parent">FrameworkElement that will hold your addition</param>
    /// <param name="child">represents the element you want to add</param>
    public static void AddUI(this FrameworkElement parent, object child) => BaseAddUI(parent, child);
    
    /// <summary>
    /// Adds created UI, to a given parent
    /// </summary>
    /// <param name="parent">GraphicalUiElement that will hold your addition</param>
    /// <param name="child">represents the element you want to add</param>
    public static void AddUI(this GraphicalUiElement parent, object child) => BaseAddUI(parent, child);

    public static void SetVisibility(this List<MenuItem> childMenuItems, ItemsControl parent, bool visible)
    {
        foreach (MenuItem child in childMenuItems)
        {
            child.IsVisible = visible;
            if (parent == null)
                continue;
            if (visible)
            {
                if (parent.Items.Contains(child))
                    continue;
                parent.Items.Add(child);
            }
            else
            {
                if (!parent.Items.Contains(child))
                    continue;
                parent.Items.Remove(child);
            }
        }
    }
}