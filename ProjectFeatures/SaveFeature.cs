using Gum.Forms.Controls;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.ProjectFeatures;

public class SaveFeature : ProjectFeature
{
    protected Project project;
    private MenuItem saveProjectButton = null!;
    
    protected delegate bool ShouldOverwriteDelegate(ref string filePath);
    protected ShouldOverwriteDelegate shouldOverwriteFilePath;

    public SaveFeature(Project project)
    {
        this.project = project;
        this.shouldOverwriteFilePath = (ref string _) => project.FilePath != string.Empty;
    }

    public override void LoadUI(MenuItem menu, bool isVisible)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;

        this.saveProjectButton = new MenuItem
        {
            Header = "Save Project",
            Height = UIParams.minButtonHeight
        };
        // UIParams.SetDefaultButton(this.saveProjectButton);

        this.saveProjectButton.Clicked += (_, _) => Save();
        this.children.Add(this.saveProjectButton);
        SetVisible(isVisible);
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