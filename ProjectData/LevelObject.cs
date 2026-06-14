using System;
using DLLevelBuilder.Window;

namespace DLLevelBuilder;

public abstract class LevelObject
{
    protected Level parentLevel;
    public Vector2 position = Vector2.Zero;
    protected Vector2 offset = Vector2.Zero;
    public bool visible = true;
    public Action onShowEvent;
    public Action onHideEvent;
    
    protected readonly WindowInstance windowRef;
    protected readonly Project projectRef;

    protected LevelObject(WindowInstance windowRef, Project projectRef, Level parentLevel)
    {
        this.windowRef = windowRef;
        this.projectRef = projectRef;
        this.parentLevel = parentLevel;
    }

    public void Show()
    {
        OnShow();
        this.visible = true;
        this.onShowEvent?.Invoke();
    }

    public void Hide()
    {
        OnHide();
        this.visible = false;
        this.onHideEvent?.Invoke();
    }
    protected abstract void OnShow();
    protected abstract void OnHide();
    public abstract void ShowSelectionVisual();
    public abstract void HideSelectionVisual();
    
    public void UpdateVisualOffset(Vector2 objectOffset)
    {
        this.offset = objectOffset;
        UpdateVisualPosition(this.windowRef.GetWindowSize());
    }
    
    public abstract void UpdateVisualPosition(Vector2 screenSize);
}