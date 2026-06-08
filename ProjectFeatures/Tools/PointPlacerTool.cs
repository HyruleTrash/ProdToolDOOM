using DLLevelBuilder.UI;
using Gum.Forms.Controls;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.ProjectFeatures.Tools;

public class PointPlacerTool : BasePlacerTool
{
    private Button button = null!;

    public PointPlacerTool() : base(Program.instance)
    {
        Texture2D pointTexture = Program.instance.Content.Load<Texture2D>("Icons/Point");
        this.toCall = () => Program.instance.cmdHistory.ApplyCmd(new AddPointCmd(Project.Instance, this.lastMousePosition, pointTexture, Program.instance));
    }

    public override void SetVisuals()
    {
        Debug.Log("PointPlacerTool::SetVisuals");
    }

    public override FrameworkElement LoadUI()
    {
        Texture2D toolTexture = Program.instance.Content.Load<Texture2D>("Tools/PointTool");
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