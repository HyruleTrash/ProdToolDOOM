using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using DLLevelBuilder.ProjectFeatures.Tools;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.ProjectFeatures;

public class ToolBarFeature(GumService gum, Project project)
{
    private StackPanel toolStack = null!;
    private Button addLevelButton = null!;
    private Button addNewEntityButton = null!;
    private Button addNewEntityToLevelButton = null!;
    private Button addPointToLevelButton = null!;

    private void AddLevel() => Program.instance.cmdHistory.ApplyCmd(new AddLevelCmd(project));
    private void AddEntityData() => EntityCreationPopup.ToggleVisibility();
    private void AddEntity() => Program.instance.toolManager?.SetTool(typeof(EntityPlacerTool));
    
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

        this.addLevelButton = new Button
        {
            Text = "Create new level",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.addLevelButton);
        this.addLevelButton.Click += (_, _) => AddLevel();
        this.toolStack.AddUI(this.addLevelButton);

        this.addNewEntityButton = new Button
        {
            Text = "Add new Entity to project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.addNewEntityButton);
        this.addNewEntityButton.Click += (_, _) => AddEntityData();
        this.toolStack.AddUI(this.addNewEntityButton);

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