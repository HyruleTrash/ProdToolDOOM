
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures;

public class EntityDataManageFeature(Project projectRef) : ProjectFeature
{
    private Button toggleManagerButton = null!;

    public override void LoadUI(object parent)
    {
        if (!ShouldLoadUI(parent))
            return;

        Program.instance.onScreenSizeChange += UpdatePosition;
        
        this.toggleManagerButton = new Button
        {
            Text = "Manage known entities",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.toggleManagerButton);
        this.toggleManagerButton.Click += (_, _) => TogglePopup();
        projectRef.ToolContainer.AddChild(this.toggleManagerButton);
        
        UpdatePosition(Program.instance.GetWindowSize());
    }

    private void UpdatePosition(Vector2 windowSize)
    {
        const float margin = 4f;
        this.toggleManagerButton.X = windowSize.x - (this.toggleManagerButton.ActualWidth + margin);
        this.toggleManagerButton.Y = margin + this.toggleManagerButton.ActualHeight / 2 + UIParams.defaultOutLineWidth + UIParams.minButtonHeight;
    }

    private static void TogglePopup() => EntityManagerPopup.ToggleVisibility();
}