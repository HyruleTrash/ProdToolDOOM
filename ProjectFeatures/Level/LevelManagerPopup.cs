using DLLevelBuilder.UI;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using Button = Gum.Forms.Controls.Button;
using Color = Microsoft.Xna.Framework.Color;
using HorizontalAlignment = RenderingLibrary.Graphics.HorizontalAlignment;

namespace DLLevelBuilder.ProjectFeatures;

public class LevelManagerPopup : Popup<LevelManagerPopup>
{
    private static readonly float scrollPadding = 16f;
    
    private readonly ColoredRectangleRuntime popupBG;
    private readonly RectangleRuntime popupBGBorder;
    private readonly ScrollViewer scrollViewer;
    private readonly StackPanel panel;
    private readonly ContainerRuntime buttonContainer;
    private readonly Button createNewButton;
    private readonly Button exportLevel;

    private List<LevelVisual> visuals = [];
    private readonly Texture2D closeIcon;
    
    public LevelManagerPopup()
    {
        Project projectRef = Project.Instance;
        this.closeIcon = Program.instance.Content.Load<Texture2D>("Icons/Cross");
        
        // scroll panel, and inner panel
        this.scrollViewer = new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden };
        this.panel = new StackPanel
        {
            Visual =
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                Width = 100,
                Height = scrollPadding
            },
            Y = scrollPadding,
            Spacing = scrollPadding
        };

        ScrollViewerVisual scrollViewerVisual = (ScrollViewerVisual)this.scrollViewer.Visual;
        scrollViewerVisual.Background.Color = Color.Transparent;
        ScrollBarVisual scrollBarVisual = scrollViewerVisual.VerticalScrollBarInstance;
        scrollBarVisual.Width = Params.minBoxSize * 0.2f;
        scrollBarVisual.DownButtonIcon.Visible = false;
        scrollBarVisual.UpButtonIcon.Visible = false;
        CustomButtonVisual.Create(scrollBarVisual.UpButtonInstance, CustomButtonVisual.CustomButtonTheme.InvertedTheme);
        CustomButtonVisual.Create(scrollBarVisual.DownButtonInstance, CustomButtonVisual.CustomButtonTheme.InvertedTheme);
        CustomButtonVisual.Create(scrollBarVisual.ThumbInstance, CustomButtonVisual.CustomButtonTheme.InvertedTheme);
        
        // buttons
        this.buttonContainer = new ContainerRuntime
        {
            HeightUnits = DimensionUnitType.RelativeToChildren,
            XUnits = GeneralUnitType.PixelsFromLarge,
            YUnits = GeneralUnitType.PixelsFromSmall,
            XOrigin = HorizontalAlignment.Right,
            YOrigin = VerticalAlignment.Top,
            // RaiseChildrenEventsOutsideOfBounds = true
        };
        
        this.createNewButton = new Button()
        {
            Text = "+",
            Height = Params.minButtonHeight,
            Width = Params.minBoxSize,
        };
        CustomButtonVisual.Create(this.createNewButton);
        this.createNewButton.Click += (_, _) => Program.instance.cmdHistory.ApplyCmd(new AddLevelCmd(projectRef));
        this.createNewButton.Anchor(Anchor.Right);
        this.createNewButton.X = 0;
        
        this.exportLevel = new Button()
        {
            Text = "Export current level",
            Height = Params.minButtonHeight,
        };
        this.exportLevel.Anchor(Anchor.Left);
        this.exportLevel.X = 0;
        CustomButtonVisual.Create(this.exportLevel);
        this.exportLevel.Click += (_, _) => Program.instance.cmdHistory.ApplyCmd(new ExportLevelCmd(projectRef, projectRef.levels[projectRef.CurrentLevel]));
        
        // Background
        this.popupBG = new ColoredRectangleRuntime { Color = Params.DefaultFillColor };
        this.popupBGBorder = new RectangleRuntime { Color = Params.DefaultOutlineColor };
        
        // structure
        this.container.AddChild(this.popupBG);
        this.container.AddChild(this.popupBGBorder);
        this.container.AddChild(this.buttonContainer);
        this.container.AddChild(this.scrollViewer.Visual);
        this.scrollViewer.AddChild(this.panel);
        this.buttonContainer.AddChild(this.createNewButton.Visual);
        this.buttonContainer.AddChild(this.exportLevel.Visual);

        Project.Instance.RegisterOnLevelsChanged(LoadLevels);
        
        UpdatePositionsAndSizes();
        LoadLevels(Project.Instance.levels);
    }

    protected override void UpdatePositionsAndSizes()
    {
        base.UpdatePositionsAndSizes();

        const float popupWidth = 400f;
        const float popupHeight = 300f;
        const float margin = 16f;

        float containerWidth = this.popUpContainerRef.Width;

        // Top-right anchor
        float popupX = containerWidth - popupWidth - margin;
        const float popupY = margin + Params.minBoxSize;

        // Background
        this.popupBG.Width = popupWidth;
        this.popupBG.Height = popupHeight;
        this.popupBG.X = popupX;
        this.popupBG.Y = popupY;

        // Background border
        this.popupBGBorder.Width = popupWidth + Params.defaultOutLineWidth;
        this.popupBGBorder.Height = popupHeight + Params.defaultOutLineWidth;
        this.popupBGBorder.X = popupX - Params.defaultOutLineWidth / 2;
        this.popupBGBorder.Y = popupY - Params.defaultOutLineWidth / 2;

        // ScrollPanel
        ScrollViewerVisual scrollViewerVisual = (ScrollViewerVisual)this.scrollViewer.Visual;
        ScrollBarVisual scrollBarVisual = scrollViewerVisual.VerticalScrollBarInstance;
        scrollBarVisual.Width = Params.minBoxSize * 0.3f;
        
        this.scrollViewer.Width = popupWidth - Params.popupPadding + scrollBarVisual.Width * 0.5f;
        this.scrollViewer.Height = popupHeight - Params.popupPadding * 1.5f + scrollBarVisual.Width * 0.5f - Params.minBoxSize;
        this.scrollViewer.X = popupX + Params.popupPadding / 2;
        this.scrollViewer.Y = popupY + Params.popupPadding + Params.minBoxSize;
        
        // instances
        foreach (LevelVisual levelVisual in this.visuals) levelVisual.UpdateVisuals();
        
        // buttons
        this.buttonContainer.Width = popupWidth - Params.popupPadding / 2 - Params.borderMargin / 2;
        this.buttonContainer.X = -Params.popupPadding;
        this.buttonContainer.Y = -Params.borderMargin;
    }
    
    private void LoadLevels(IReadOnlyDictionary<int, Level> data)
    {
        List<LevelVisual> upToDateVisuals = [];
        foreach ((int id, var level) in data)
        {
            if (level == null)
                continue;
            LevelVisual? instance = this.visuals.FirstOrDefault(x => x.id == id);
            if (instance != null)
            {
                instance.UpdateVisuals();
                upToDateVisuals.Add(instance);
                continue;
            }

            instance = this.visuals.FirstOrDefault(x => x.id == null);
            if (instance == null)
            {
                instance = new LevelVisual(this.closeIcon, this.panel, id, level);
                this.visuals.Add(instance);
                upToDateVisuals.Add(instance);
                continue;
            }

            instance.levelData = level;
            instance.id = id;
            instance.UpdateVisuals();
            upToDateVisuals.Add(instance);
        }

        foreach (LevelVisual entityDataVisual in this.visuals.Where(entityDataVisual => !upToDateVisuals.Contains(entityDataVisual)))
            entityDataVisual.RemoveAndHide();
    }
}