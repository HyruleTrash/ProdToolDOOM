using System;
using System.Text;
using DLLevelBuilder.UI;
using Gum.Forms.Controls;

namespace DLLevelBuilder.ProjectFeatures.Exporting;

public class ExportFeature(Project project) : ProjectFeature
{
    private MenuItem exportButton = null!;

    public override void LoadUI(MenuItem menu, bool isVisible = true)
    {
        if (!ShouldLoadUI(menu))
            return;
        this.parent = menu;
        
        this.exportButton = new MenuItem
        {
            Header = "Export Level",
            Height = Params.minButtonHeight
        };
        CustomMenuItemVisual.Create(this.exportButton);
        this.exportButton.Clicked += (_, _) => Export();
        this.children.Add(this.exportButton);
        SetVisible(isVisible);
    }

    private void Export() => Program.instance.cmdHistory.ApplyCmd(new ExportLevelCmd(Project.Instance, project.levels[project.CurrentLevel]));
}