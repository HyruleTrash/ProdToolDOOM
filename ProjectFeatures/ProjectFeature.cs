using System.Collections.Generic;
using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;

namespace DLLevelBuilder.ProjectFeatures;

public abstract class ProjectFeature
{
    protected MenuItem? parent;
    protected readonly List<MenuItem> children = [];
    
    /// <summary>
    /// Function that gets overwritten by feature to load ui
    /// </summary>
    /// <param name="menu">GraphicalUiElement or FrameworkElement that all loaded ui will get parented to</param>
    public abstract void LoadUI(MenuItem menu, bool isVisible);
    
    protected bool ShouldLoadUI(object? parent) => 
        parent != null || parent is GraphicalUiElement || parent is FrameworkElement;

    public virtual void SetVisible(bool state)
    {
        foreach (MenuItem child in this.children)
        {
            child.IsVisible = state;
            if (this.parent == null)
                continue;
            if (state)
            {
                if (this.parent.Items.Contains(child))
                    continue;
                this.parent.Items.Add(child);
            }
            else
            {
                if (!this.parent.Items.Contains(child))
                    continue;
                this.parent.Items.Remove(child);
            }
        }
    }
}