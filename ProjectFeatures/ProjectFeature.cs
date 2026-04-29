using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;

namespace ProdToolDOOM.ProjectFeatures;

public abstract class ProjectFeature
{
    /// <summary>
    /// Function that gets overwritten by feature to load ui
    /// </summary>
    /// <param name="parent">GraphicalUiElement or FrameworkElement that all loaded ui will get parented to</param>
    public abstract void LoadUI(object parent);
    
    protected bool ShouldLoadUI(object? parent)
    {
        return parent == null || parent is GraphicalUiElement p || parent is FrameworkElement f;
    }
    
    /// <summary>
    /// Adds created UI, to a given parent
    /// </summary>
    /// <param name="parent">GraphicalUiElement or FrameworkElement that will hold your addition</param>
    /// <param name="child">represents the element you want to add</param>
    protected static void AddUI(object parent, object child)
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
}