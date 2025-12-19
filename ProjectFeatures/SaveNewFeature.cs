using Gum.Forms.DefaultVisuals;
using Button = Gum.Forms.Controls.Button;
using static Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM.ProjectFeatures;

public class SaveNewFeature : SaveFeature
{
    private Button saveProjectAsButton = null!;

    public SaveNewFeature(Project project) : base(project)
    {
        shouldOverwriteFilePath = ShouldOverwriteFilePath;
    }
    
    public override void LoadUI(object? parent)
    {
        if (!ShouldLoadUI(parent))
            return;
        
        saveProjectAsButton = new Button
        {
            Text = "New Project",
            Height = UIParams.minButtonHeight
        };
        UIParams.SetDefaultButton((ButtonVisual)saveProjectAsButton.Visual);
        
        saveProjectAsButton.Click += (_, _) => Save();
        project.filePathChanged += (newPath) =>
        {
            saveProjectAsButton.Text = newPath == string.Empty ? "New Project" : "Save Project as...";
        };
        AddUI(parent, saveProjectAsButton);
    }

    private bool ShouldOverwriteFilePath(ref string tempPath)
    {
        if (project.FilePath == string.Empty)
        {
            tempPath = FileExplorerHelper.SaveWithFileExplorer();
            Debug.Log(tempPath);
        }else
            tempPath = FileExplorerHelper.SaveWithFileExplorer(tempPath);

        return true;
    }
}