using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Wireframe;
using MonoGameGum;
using ProdToolDOOM.ProjectFeatures.Tools;
using Button = Gum.Forms.Controls.Button;
using static Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM.ProjectFeatures;

public class ToolBarFeature(GumService gum, Project project) : ProjectFeature
{
    private StackPanel toolStack = null!;
    private Button addLevelButton = null!;
    private Button addNewEntityButton = null!;
    private Button addNewEntityToLevelButton = null!;
    private Button addPointToLevelButton = null!;

    private void AddLevel() => Program.instance.cmdHistory.ApplyCmd(new AddLevelCmd(project));
    private void AddEntityData() => Program.instance.cmdHistory.ApplyCmd(new AddEntityDataCmd(project));
    private void AddEntity() => Program.instance.toolManager?.SetTool(typeof(EntityPlacerTool));
    
    private static void SetToolToPointPlacer() => Program.instance.toolManager?.SetTool(typeof(PointPlacerTool));
    
    public override void LoadUI(object parent)
    {
        if (!ShouldLoadUI(parent))
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
        AddUI(parent, this.toolStack);

        this.addLevelButton = new Button
        {
            Text = "Create new level",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.addLevelButton);
        this.addLevelButton.Click += (_, _) => AddLevel();
        AddUI(this.toolStack, this.addLevelButton);

        this.addNewEntityButton = new Button
        {
            Text = "Add new Entity to project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.addNewEntityButton);
        this.addNewEntityButton.Click += (_, _) => AddEntityData();
        AddUI(this.toolStack, this.addNewEntityButton);

        this.addNewEntityToLevelButton = new Button
        {
            Text = "Add Entity to level",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.addNewEntityToLevelButton);
        this.addNewEntityToLevelButton.Click += (_, _) => AddEntity();
        AddUI(this.toolStack, this.addNewEntityToLevelButton);

        this.addPointToLevelButton = new Button
        {
            Text = "Add point",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.addPointToLevelButton);
        this.addPointToLevelButton.Click += (_, _) => SetToolToPointPlacer();
        AddUI(this.toolStack, this.addPointToLevelButton);
    }
}