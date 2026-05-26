namespace DLLevelBuilder;

public abstract class LevelObject
{
    public Vector2 position;
    public bool visible = true;
    public Action onShowEvent;
    public Action onHideEvent;

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
}