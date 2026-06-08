using System.Collections.Generic;
using DLLevelBuilder.UI;
using Gum.Forms.Controls;
using Gum.Wireframe;

namespace DLLevelBuilder.ProjectFeatures;

public abstract class ProjectFeature
{
    protected MenuItem? parent;
    protected readonly List<MenuItem> children = [];
    
    /// <summary>
    /// Function that gets overwritten by feature to load ui
    /// </summary>
    /// <param name="menu">GraphicalUiElement or FrameworkElement that all loaded ui will get parented to</param>
    public abstract void LoadUI(MenuItem menu, bool isVisible = true);
    
    protected static bool ShouldLoadUI(object? parent) => 
        parent != null || parent is GraphicalUiElement || parent is FrameworkElement;

    public virtual void SetVisible(bool state)
    {
        if (this.parent == null) return;
        this.children.SetVisibility(this.parent, state);
    }
}