using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using DLLevelBuilder.ProjectFeatures.Tools;
using DLLevelBuilder.UI;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.ProjectFeatures;

public class ToolBarFeature(GumService gum)
{
    private StackPanel toolStack = null!;
    
    public void LoadUI(ContainerRuntime container)
    {
        if (!ShouldLoadUI(container))
            return;
        this.toolStack = new StackPanel
        {
            Visual =
            {
                ChildrenLayout = Gum.Managers.ChildrenLayout.LeftToRightStack,
                StackSpacing = 4
            },
            X = 5,
            Y = gum.CanvasHeight - Params.borderPadding,
        };
        this.toolStack.Anchor(Anchor.BottomLeft);
        container.AddUI(this.toolStack);
        
        ITool? entityPlacerTool = ToolManager.GetTool(typeof(EntityPlacerTool));
        if (entityPlacerTool != null)
            this.toolStack.AddUI(entityPlacerTool.LoadUI());

        ITool? placerTool = ToolManager.GetTool(typeof(PointPlacerTool));
        if (placerTool != null)
            this.toolStack.AddUI(placerTool.LoadUI());
    }
    
    private bool ShouldLoadUI(object? parent) => 
        parent == null || parent is GraphicalUiElement || parent is FrameworkElement;
}