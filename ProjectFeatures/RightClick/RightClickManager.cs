using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures;

public class RightClickManager : IBaseUpdatable
{
    public static RightClickManager instance = Program.instance.rightClickManager;
    public class RightClickOption(string text, Action toCall, Func<bool> shouldBeVisible = null)
    {
        public readonly Func<bool> shouldBeVisible = shouldBeVisible;
        public readonly string text = text;
        public readonly Action toCall = toCall;
        public Button button;
    }

    private class RightClickStack(RightClickOption[] options)
    {
        public RightClickOption[] options = options;
        public StackPanel rightClickOptionStack;
    }
    
    private Dictionary<Type, RightClickStack> registry = new();
    
    public struct RightClickVisualSetCall(Type givenType, Vector2 position, object currentSelection, int givenPriority)
    {
        public readonly Type type = givenType;
        public readonly Vector2 position = position;
        public readonly int priority = givenPriority;
        public readonly object currentSelection = currentSelection;
    }
    private List<RightClickVisualSetCall> visualSetCalls = [];
    public RightClickVisualSetCall? currentVisual;
    private RightClickStack? currentStack;

    private ContainerRuntime rightClickPopUp;
    private SelectionBox selectionBox;

    public RightClickManager()
    {
        this.rightClickPopUp = new ContainerRuntime
        {
            IgnoredByParentSize = true,
            Visible = false
        };
        this.rightClickPopUp.AddToRoot();
    }
    
    public bool AddOptions<T>(RightClickOption[] options)
    {
        if (this.registry.ContainsKey(typeof(T)))
            return false;

        RightClickStack entry = new(options)
        {
            rightClickOptionStack = new StackPanel()
        };

        foreach (RightClickOption option in options)
        {
            option.button = new Button
            {
                Text = option.text,
            };
            option.button.Click += (_, _) => option.toCall.Invoke();
            UIParams.SetDefaultButton(option.button);
            entry.rightClickOptionStack.AddChild(option.button);
        }

        this.registry.Add(typeof(T), entry);
        return true;
    }

    public void ShowOptions<T>(Vector2 position, object selection, int priority) => 
        this.visualSetCalls.Add(new RightClickVisualSetCall(typeof(T), position, selection, priority));

    public void HideOptions<T>()
    {
        this.visualSetCalls.RemoveAll(x => x.type == typeof(T));
        if (this.currentVisual?.type == typeof(T)) Reset();
    }

    public void Update(float dt, WindowInstance windowRef)
    {
        MouseState mouseState = windowRef.Mouse.currentMouseState;
        ShowHighestPriority();
        if (!this.rightClickPopUp.Visible || !this.selectionBox.IsInsideBounds(new Vector2(mouseState.X, mouseState.Y)))
            Reset();
    }

    private void ShowHighestPriority()
    {
        RightClickVisualSetCall? newVisual = GetHighestPriority();
        
        if (newVisual == null || !this.registry.TryGetValue(newVisual.Value.type, out RightClickStack? stack))
        {
            Reset();
            return;
        }
        
        // overkill check because the damned thing didn't wanna listen, 'stupid dog, you make me look bad!'
        bool isSame =
            this.currentVisual.HasValue &&
            this.currentVisual.Value.type == newVisual.Value.type &&
            this.currentVisual.Value.position == newVisual.Value.position &&
            this.currentVisual.Value.priority == newVisual.Value.priority &&
            Equals(this.currentVisual.Value.currentSelection, newVisual.Value.currentSelection);

        if (!isSame)
        {
            if (this.currentStack != null) 
                this.currentStack.rightClickOptionStack.Visual.Parent = null;
            
            this.currentVisual = newVisual;
            this.currentStack = stack;

            this.rightClickPopUp.X = newVisual.Value.position.x;
            this.rightClickPopUp.Y = newVisual.Value.position.y;
            this.rightClickPopUp.Visible = true;
            this.rightClickPopUp.AddChild(stack.rightClickOptionStack);
            this.currentStack = stack;

            Vector2 size = new(this.rightClickPopUp.GetAbsoluteWidth(), this.rightClickPopUp.GetAbsoluteHeight());
            Vector2 offset = new(UIParams.minNearSelection, UIParams.minNearSelection);
            this.selectionBox = new SelectionBox(newVisual.Value.position + (size / 2), size + offset);
        }
        
        UpdateOptionVisibility(stack);
    }

    private void UpdateOptionVisibility(RightClickStack stack)
    {
        foreach (RightClickOption option in stack.options) option.button.Visual.Visible = option.shouldBeVisible?.Invoke() ?? true;
    }

    private RightClickVisualSetCall? GetHighestPriority()
    {
        RightClickVisualSetCall? highestPriority = null;
        foreach (RightClickVisualSetCall setCall in this.visualSetCalls)
        {
            if (highestPriority == null)
            {
                highestPriority = setCall;
                continue;
            }
            if (highestPriority.Value.priority < setCall.priority)
            {
                highestPriority = setCall;
            }
        }
        return highestPriority;
    }

    public void Reset()
    {
        if (this.currentStack != null)
            this.currentStack.rightClickOptionStack.Visual.Parent = null;
        this.rightClickPopUp.Children?.Clear();
        this.rightClickPopUp.Visible = false;
        this.currentStack = null;
        this.currentVisual = null;
        this.visualSetCalls.Clear();
    }
}