using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using DLLevelBuilder.ProjectFeatures.Tools;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.ProjectFeatures;

public class ToolBarFeature(GumService gum)
{
    private StackPanel toolStack = null!;
    private Button addNewEntityToLevelButton = null!;
    private Button addPointToLevelButton = null!;
    
    private static void AddEntity() => Program.instance.toolManager?.SetTool(typeof(EntityPlacerTool));
    
    private static void SetToolToPointPlacer() => Program.instance.toolManager?.SetTool(typeof(PointPlacerTool));
    
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
            Y = gum.CanvasHeight - UIParams.borderPadding,
        };
        this.toolStack.Anchor(Anchor.BottomLeft);
        container.AddUI(this.toolStack);

        this.addNewEntityToLevelButton = new Button
        {
            Text = "Add Entity to level",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.addNewEntityToLevelButton);
        this.addNewEntityToLevelButton.Click += (_, _) => AddEntity();
        this.toolStack.AddUI(this.addNewEntityToLevelButton);

        this.addPointToLevelButton = new Button
        {
            Text = "Add point",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.addPointToLevelButton);
        this.addPointToLevelButton.Click += (_, _) => SetToolToPointPlacer();
        this.toolStack.AddUI(this.addPointToLevelButton);
    }
    
    private bool ShouldLoadUI(object? parent) => 
        parent == null || parent is GraphicalUiElement || parent is FrameworkElement;
}