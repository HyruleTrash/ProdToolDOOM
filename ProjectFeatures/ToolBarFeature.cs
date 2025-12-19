using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Wireframe;
using MonoGameGum;
using Button = Gum.Forms.Controls.Button;
using static Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM.ProjectFeatures;

public class ToolBarFeature(GumService gum, Project project) : ProjectFeature
{
    private StackPanel toolStack = null!;
    private Button addLevelButton = null!;
    private Button addNewEntityButton = null!;
    private Button addNewEntityToLevelButton = null!;
    
    private void AddLevel() => Program.instance.cmdHistory.ApplyCmd(new AddLevelCmd(project));
    private void AddEntityData() => Program.instance.cmdHistory.ApplyCmd(new AddEntityDataCmd(project));
    private void AddEntity() => Program.instance.cmdHistory.ApplyCmd(new AddEntityCmd(project));
    
    public override void LoadUI(object? parent)
    {
        if (!ShouldLoadUI(parent))
            return;
        toolStack = new StackPanel
        {
            Visual =
            {
                ChildrenLayout = Gum.Managers.ChildrenLayout.LeftToRightStack,
                StackSpacing = 4
            },
            X = 5,
            Y = gum.CanvasHeight - UIParams.borderPadding,
        };
        toolStack.Anchor(Anchor.BottomLeft);
        AddUI(parent, toolStack);
        
        addLevelButton = new Button
        {
            Text = "Create new level",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton((ButtonVisual)addLevelButton.Visual);
        addLevelButton.Click += (_, _) => AddLevel();
        AddUI(toolStack, addLevelButton);

        addNewEntityButton = new Button
        {
            Text = "Add new Entity to project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton((ButtonVisual)addNewEntityButton.Visual);
        addNewEntityButton.Click += (_, _) => AddEntityData();
        AddUI(toolStack, addNewEntityButton);

        addNewEntityToLevelButton = new Button
        {
            Text = "Add Entity to level",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton((ButtonVisual)addNewEntityToLevelButton.Visual);
        addNewEntityToLevelButton.Click += (_, _) => AddEntity();
        AddUI(toolStack, addNewEntityToLevelButton);
    }
}