using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;

namespace DLLevelBuilder.ProjectFeatures;

public abstract class ProjectFeature
{
    /// <summary>
    /// Function that gets overwritten by feature to load ui
    /// </summary>
    /// <param name="menu">GraphicalUiElement or FrameworkElement that all loaded ui will get parented to</param>
    public abstract void LoadUI(MenuItem menu);
    
    protected bool ShouldLoadUI(object? parent) => 
        parent != null || parent is GraphicalUiElement || parent is FrameworkElement;

    public abstract void SetVisible(bool state);
}