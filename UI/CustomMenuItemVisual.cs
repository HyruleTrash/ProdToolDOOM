using System.Reflection;
using Gum.Converters;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Button = Gum.Forms.Controls.Button;

namespace DLLevelBuilder.UI;

public class CustomMenuItemVisual
{
    private static readonly Dictionary<MenuItem, CustomMenuItemVisual> CustomMenuItem = [];
    private readonly MenuItem menuItem;
    private readonly RectangleRuntime outline;
    private readonly MenuItemVisual visual;
    private SpriteRuntime? icon;
    private ScrollViewerVisual? scrollViewVisual;

    private CustomMenuItemVisual(MenuItem menuItem)
    {
        this.menuItem = menuItem;
        this.visual = (MenuItemVisual)menuItem.Visual;
        
        this.outline = new RectangleRuntime
        {
            X = 0,
            Y = 0,
            XUnits = GeneralUnitType.PixelsFromMiddle,
            YUnits = GeneralUnitType.PixelsFromMiddle,
            XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Center,
            YOrigin = VerticalAlignment.Center,
            Width = 0,
            Height = 0,
            WidthUnits = DimensionUnitType.RelativeToParent,
            HeightUnits = DimensionUnitType.RelativeToParent, 
            Color = Params.DefaultOutlineColor,
            LineWidth = Params.defaultOutLineWidth,
            Visible = this.visual.Visible,
            IgnoredByParentSize = true
        };
        menuItem.AddChild(this.outline);
        
        menuItem.ItemsCollectionChanged += (_, _) => SetScrollView();
        
        CustomMenuItem.Add(menuItem, this);
        this.visual.ParentChanged += (_, _) =>
        {
            if (this.visual.Parent == null) // removed / disposed
                CustomMenuItem.Remove(menuItem);
        };
        
        menuItem.Clicked += (_, _) => UiInputGuard.Lock(TimeSpan.FromMilliseconds(Params.clickCaptureMs), Program.instance.gameTime);

        CheckMinSizes();
        SetScrollView();
    }

    private void CheckMinSizes()
    {
        CheckMinHeight();
        CheckMinWidth();
    }

    private void CheckMinHeight()
    {
        this.visual.HeightUnits = DimensionUnitType.RelativeToChildren;
        this.visual.UpdateLayout();

        if (!(this.visual.ContainerInstance.GetAbsoluteHeight() < Params.minBoxSize)) return;
        this.visual.HeightUnits = DimensionUnitType.Absolute;
        this.menuItem.Height = Params.minBoxSize;
        this.visual.ContainerInstance.YOrigin = VerticalAlignment.Center;
        this.visual.ContainerInstance.YUnits = GeneralUnitType.PixelsFromMiddle;
        this.visual.ContainerInstance.Y = -2;
    }

    private void CheckMinWidth()
    {
        this.visual.WidthUnits = DimensionUnitType.RelativeToChildren;
        this.visual.UpdateLayout();

        if (!(this.visual.ContainerInstance.GetAbsoluteWidth() < Params.minBoxSize)) return;
        this.visual.WidthUnits = DimensionUnitType.Absolute;
        this.menuItem.Width = Params.minBoxSize;
    }

    private void SetScrollView()
    {
        if (!this.menuItem.IsPopupVisible) return;
        FieldInfo? fieldInfo = typeof(MenuItem).GetField("itemsPopup", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo == null || fieldInfo.GetValue(this.menuItem) is not ScrollViewer scrollViewer) return;
        this.scrollViewVisual = (ScrollViewerVisual)scrollViewer.Visual;
        this.scrollViewVisual.Background.Color = Color.Transparent;
        this.scrollViewVisual.Background.Visible = false;
    }

    private void SetIcon(SpriteRuntime sprite) => this.icon = sprite;

    private void BaseState()
    {
        this.visual.Background.ApplyState(Styling.ActiveStyle.NineSlice.Solid);
        this.visual.Background.Visible = true;
        CheckMinSizes();
    }

    private void EnabledState()
    {
        BaseState();
        this.visual.Background.Color = Params.DefaultFillColor;
        this.outline.Color = Params.DefaultOutlineColor;
        this.outline.Visible = true;
        this.visual.TextInstance.Color = Params.DefaultOutlineColor;
        if (this.icon != null) this.icon.Color = Params.DefaultOutlineColor;
    }

    private void HighlightedState()
    {
        BaseState();
        this.visual.Background.Color = Params.DefaultOutlineColor;
        this.outline.Color = Params.DefaultFillColor;
        this.visual.TextInstance.Color = Params.DefaultFillColor;
        if (this.icon != null) this.icon.Color = Params.DefaultFillColor;
    }

    private void SelectedState()
    {
        BaseState();
        this.visual.Background.Color = Params.CanvasColor;
        this.outline.Color = Params.DefaultOutlineColor;
        this.visual.TextInstance.Color = Params.DefaultOutlineColor;
        SetScrollView();
        
        if (this.scrollViewVisual == null)
            return;
        float biggestWidth = this.scrollViewVisual.InnerPanelInstance.GetAbsoluteWidth();
        foreach (MenuItem item in this.menuItem.Items)
        {
            MenuItemVisual v = (MenuItemVisual)item.Visual;
            float w = v.GetAbsoluteWidth();
            if (w < biggestWidth)
                v.Width += biggestWidth - w;
        }
    }

    public static void Create(MenuItem menuItem)
    {
        CustomMenuItemVisual customVisual = new(menuItem);
        MenuItemVisual visual = (MenuItemVisual)menuItem.Visual;
        
        StateSave enabled = visual.States.Enabled;
        enabled.Clear();
        enabled.Apply = customVisual.EnabledState;
        
        StateSave selected = visual.States.Selected;
        selected.Clear();
        selected.Apply = customVisual.SelectedState;
        
        StateSave highlighted = visual.States.Highlighted;
        highlighted.Clear();
        highlighted.Apply = customVisual.HighlightedState;
        
        visual.ApplyState(enabled);
    }

    public static void AddIcon(MenuItem menuItem, Texture2D iconTex)
    {
        menuItem.Header = "";
        SpriteRuntime icon = new()
        {
            Texture = iconTex,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = iconTex.Width,
            TextureHeight = iconTex.Height,
            IgnoredByParentSize = true
        };
        icon.Anchor(Anchor.Center);
        menuItem.AddChild(icon);

        if (!CustomMenuItem.TryGetValue(menuItem, out CustomMenuItemVisual? menuItemVisual)) return;
        menuItemVisual.SetIcon(icon);
        MenuItemVisual visual = (MenuItemVisual)menuItem.Visual;
        visual.HeightUnits = DimensionUnitType.ScreenPixel;
        visual.Background.ApplyState(visual.States.Enabled);
    }
}