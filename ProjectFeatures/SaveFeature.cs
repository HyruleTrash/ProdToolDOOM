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
        shouldOverwriteFilePath = (ref string _) => project.FilePath != string.Empty;
    }

    public override void LoadUI(object? parent)
    {
        if (!ShouldLoadUI(parent))
            return;
        
        saveProjectButton = new Button
        {
            Text = "Save Project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton((ButtonVisual)saveProjectButton.Visual);
        
        saveProjectButton.Click += (_, _) => Save();
        AddUI(parent, saveProjectButton);
    }

    protected void Save()
    {
        if (project.CheckSaveStrategy())
            return;
        Debug.Log("Saving project file...");

        var tempPath = project.FilePath;
        if (!shouldOverwriteFilePath.Invoke(ref tempPath))
            return;

        if (project.saveStrat.Save(tempPath))
        {
            project.FilePath = tempPath;
        }
    }
}