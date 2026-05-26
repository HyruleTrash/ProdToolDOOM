using Gum.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.GueDeriving;
using DLLevelBuilder.ProjectFeatures;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;

namespace DLLevelBuilder;

public class Entity : LevelObject, IDisposable, IBaseUpdatable
{
    public int LevelId { get; set; }
    public int LevelObjectId { get; set; }
    public Vector2 Position { get => this.position; set => this.position = value; }
    public int DataId { get; set; }

    public SpriteRuntime? icon;
    private TextRuntime? nameText;
    private SpriteRuntime? selectedIcon;
    private ContainerRuntime? iconContainer;
    
    private readonly Texture2D entityTextureRef;
    private bool beingMoved = false;
    private Vector2 oldPosition;
    
    private readonly WindowInstance windowRef;
    private readonly Project projectRef;

    public Entity(int levelId, Texture2D entityTexture, WindowInstance windowRef, Project projectRef,
        int levelObjectId = -1, Vector2? position = null, int dataId = -1)
    {
        this.LevelId = levelId;
        this.LevelObjectId = levelObjectId;
        if (position is not null) 
            this.position = position;
        
        this.entityTextureRef = entityTexture;
        this.windowRef = windowRef;
        this.projectRef = projectRef;

        this.DataId = dataId;
    }
    
    public Entity(Entity other, Texture2D entityTexture)
    {
        this.LevelId = other.LevelId;
        this.LevelObjectId = other.LevelObjectId;
        this.position = other.position;
        
        this.entityTextureRef = entityTexture;
        this.windowRef = other.windowRef;
        this.projectRef = other.projectRef;
    }

    public void Init()
    {
        this.iconContainer = new ContainerRuntime
        {
            Width = this.entityTextureRef.Width,
            Height = this.entityTextureRef.Height,
            IgnoredByParentSize = true
        };
        this.icon = new SpriteRuntime
        {
            Texture = this.entityTextureRef,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = this.entityTextureRef.Width,
            TextureHeight = this.entityTextureRef.Height,
            IgnoredByParentSize = true,
            Visible = this.projectRef.CurrentLevel == this.LevelId
        };
        this.iconContainer.AddChild(this.icon);
        this.selectedIcon = new SpriteRuntime
        {
            Texture = this.entityTextureRef,
            TextureAddress = TextureAddress.Custom,
            TextureWidth = this.entityTextureRef.Width,
            TextureHeight = this.entityTextureRef.Height,
            IgnoredByParentSize = true,
            Visible = false,
            Color = Color.Blue
        };
        this.iconContainer.AddChild(this.selectedIcon);
        
        this.nameText = new TextRuntime { Width = 200f };
        UpdateName();
        
        if (this.icon.Visible)
        {
            this.projectRef.canvasContainer.AddChild(this.iconContainer);
            this.projectRef.canvasContainer.AddChild(this.nameText);
        }

        this.iconContainer.RightClick += HandleRightClick;
        this.iconContainer.Dragging += HandleLeftClickHold;

        UpdateVisualPosition(this.windowRef.GetWindowSize());
        this.windowRef.onScreenSizeChange += UpdateVisualPosition;

        this.projectRef.onCurrentLevelChanged += OnLevelChanged;
    }

    public void UpdateName()
    {
        if (this.nameText != null) this.nameText.Text = this.projectRef.TryGetEntityName(this.DataId) ?? "Unnamed";
    }

    private void OnLevelChanged(int newLevelId)
    {
        if (newLevelId != this.LevelId)
        {
            if (this.icon != null) this.icon.Visible = false;
            if (this.nameText != null) this.nameText.Visible = false;
            return;
        }

        if (this.iconContainer is { Parent: null }) 
            this.projectRef.canvasContainer.AddChild(this.iconContainer);
        if (this.nameText is { Parent: null }) 
            this.projectRef.canvasContainer.AddChild(this.nameText);
        if (this.icon != null) this.icon.Visible = true;
    }

    private void HandleLeftClickHold(object? _, EventArgs __)
    {
        if (this.beingMoved) return;
        this.beingMoved = true;
        this.windowRef.Mouse.IsDragging = true;
        this.oldPosition = this.Position;
        Program.instance.UpdateRegister.Add(this);
    }
    
    private void HandleRightClick(object? _, EventArgs __) => 
        RightClickManager.instance.ShowOptions<Entity>(new Vector2(this.windowRef.Mouse.currentMouseState.Position), this, 1);

    public void UpdateVisualPosition(Vector2 screenSize)
    {
        if (this.icon == null) return;
        
        if (this.iconContainer != null)
        {
            this.iconContainer.X = this.Position.x - (float)this.entityTextureRef.Width / 2 + screenSize.x / 2;
            this.iconContainer.Y = this.Position.y - (float)this.entityTextureRef.Height / 2 + screenSize.y / 2;
        }

        if (this.nameText == null) return;
        this.nameText.X = this.Position.x - this.nameText.Width / 2 + screenSize.x / 2;
        this.nameText.Y = this.Position.y - this.nameText.Height / 2 + screenSize.y / 2;
    }
    
    public void Update(float dt, WindowInstance _)
    {
        if (!this.beingMoved)
        {
            Program.instance.UpdateRegister.Remove(this);
            this.windowRef.Mouse.IsDragging = false;
            Program.instance.cmdHistory.ApplyCmd(new MoveEntityCmd(this, new Vector2(this.oldPosition), new Vector2(this.Position)));
            return;
        }
        MouseState mouse = this.windowRef.Mouse.currentMouseState;
        if (mouse.LeftButton == ButtonState.Released) this.beingMoved = false;
        this.Position = this.windowRef.Mouse.GetMousePosition();
        UpdateVisualPosition(this.windowRef.GetWindowSize());
    }

    public void Dispose()
    {
        if (this.iconContainer != null) this.iconContainer.Parent = null;
        if (this.nameText != null) this.nameText.Parent = null;
    }

    protected override void OnShow()
    {
        if (this.icon != null) this.icon.Visible = true;
        if (this.nameText != null) this.nameText.Visible = true;
    }

    protected override void OnHide()
    {
        if (this.selectedIcon != null) this.selectedIcon.Visible = false;
        if (this.icon != null) this.icon.Visible = false;
        if (this.nameText != null) this.nameText.Visible = false;
    }

    public override void ShowSelectionVisual()
    {
        if (this.selectedIcon != null) this.selectedIcon.Visible = true;
    }

    public override void HideSelectionVisual()
    {
        if (this.selectedIcon != null) this.selectedIcon.Visible = false;
    }
}