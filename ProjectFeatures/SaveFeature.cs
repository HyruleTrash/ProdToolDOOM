using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using static Microsoft.Xna.Framework.Color;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures;

public class SaveFeature : ProjectFeature
{
    protected Project project;
    private Button saveProjectButton = null!;
    
    protected delegate bool ShouldOverwriteDelegate(ref string filePath);
    protected ShouldOverwriteDelegate shouldOverwriteFilePath;

    public SaveFeature(Project project)
    {
        this.project = project;
        this.shouldOverwriteFilePath = (ref string _) => project.FilePath != string.Empty;
    }

    public override void LoadUI(object parent)
    {
        if (!ShouldLoadUI(parent))
            return;

        this.saveProjectButton = new Button
        {
            Text = "Save Project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton(this.saveProjectButton);

        this.saveProjectButton.Click += (_, _) => Save();
        AddUI(parent, this.saveProjectButton);
    }

    protected void Save()
    {
        if (this.project.CheckSaveStrategy())
            return;
        Debug.Log("Saving project file...");

        string tempPath = this.project.FilePath;
        if (!this.shouldOverwriteFilePath.Invoke(ref tempPath))
            return;

        if (this.project.saveStrat.Save(tempPath)) 
            this.project.FilePath = tempPath;
    }
}