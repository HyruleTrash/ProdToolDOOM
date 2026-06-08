using DLLevelBuilder.UI;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Graphics;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.ProjectFeatures.Tools;

public class EntityPlacerTool : BasePlacerTool
{
    private Button button = null!;

    public EntityPlacerTool() : base(Program.instance)
    {
        Texture2D entityTexture = Program.instance.Content.Load<Texture2D>("Icons/Entity");
        this.toCall = () => Program.instance.cmdHistory.ApplyCmd(new AddEntityCmd(Project.Instance, this.lastMousePosition, entityTexture, Program.instance));
    }

    public override void SetVisuals()
    {
        Debug.Log("EntityPlacerTool::SetVisuals");
    }

    public override FrameworkElement LoadUI()
    {
        Texture2D toolTexture = Program.instance.Content.Load<Texture2D>("Tools/EntityTool");
        this.button = new Button
        {
            Text = "",
            Width = toolTexture.Width,
            Height = toolTexture.Height,
        };
        CustomButtonVisual.Create(this.button);
        this.button.Click += (_, _) => ToolManager.SetTool(GetType());
        CustomButtonVisual.AddIcon(this.button, toolTexture);
        return this.button;
    }
}