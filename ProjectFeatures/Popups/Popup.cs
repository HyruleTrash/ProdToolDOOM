using MonoGameGum.GueDeriving;
using Color = Microsoft.Xna.Framework.Color;

namespace ProdToolDOOM.ProjectFeatures;

public abstract class Popup<T> where T : Popup<T>, new()
{
    public static T Instance
    {
        get
        {
            instance ??= new T();
            return instance;
        }
        protected set => instance = value;
    }
    private static T? instance;
    
    protected readonly ContainerRuntime popUpContainerRef;
    protected readonly ContainerRuntime container;
    private readonly ColoredRectangleRuntime transparentOverlay;

    protected Popup()
    {
        this.popUpContainerRef = Project.instance.popUpContainer;
        this.container = new ContainerRuntime { Visible = false };

        Color transparentBlack = Color.Black;
        transparentBlack.A = 50;
        this.transparentOverlay = new ColoredRectangleRuntime { Color = transparentBlack };

        this.popUpContainerRef.AddChild(this.container);
        this.container.AddChild(this.transparentOverlay);
        
        this.popUpContainerRef.SizeChanged += (_, _) => UpdatePositionsAndSizes();
        this.container.Click += (_, _) => ToggleVisibility();
    }

    protected virtual void UpdatePositionsAndSizes()
    {
        this.container.Width = this.popUpContainerRef.Width;
        this.container.Height = this.popUpContainerRef.Height;
        this.transparentOverlay.Width = this.popUpContainerRef.Width;
        this.transparentOverlay.Height = this.popUpContainerRef.Height;
    }

    public static void ToggleVisibility() => Instance.container.Visible = !Instance.container.Visible;
}