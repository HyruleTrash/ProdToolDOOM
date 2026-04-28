using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures;

public class rightClickManager : IBaseUpdatable
{
    public static rightClickManager instance = Program.instance.rightClickManager;
    public class RightClickOption(string text, Action toCall)
    {
        public string text = text;
        public Action toCall = toCall;
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

    public rightClickManager()
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

    public void ShowOptions<T>(Vector2 position, object selection, int priority)
    {
        this.visualSetCalls.Add(new RightClickVisualSetCall(typeof(T), position, selection, priority));
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
        if (this.currentVisual != null) return;
        this.currentVisual = GetHighestPriority();
        if (this.currentVisual == null) return;
        
        if (this.currentStack != null) this.currentStack.rightClickOptionStack.Visual.Parent = null;
        if (!this.registry.TryGetValue(this.currentVisual.Value.type, out RightClickStack? stack))
            return;

        this.rightClickPopUp.X = this.currentVisual.Value.position.x;
        this.rightClickPopUp.Y = this.currentVisual.Value.position.y;
        this.rightClickPopUp.Visible = true;
        this.rightClickPopUp.AddChild(stack.rightClickOptionStack);
        this.currentStack = stack;

        Vector2 size = new(this.rightClickPopUp.GetAbsoluteWidth(), this.rightClickPopUp.GetAbsoluteHeight());
        Vector2 offset = new(UIParams.minNearSelection, UIParams.minNearSelection);
        this.selectionBox = new SelectionBox(this.currentVisual.Value.position + (size / 2), size + offset);
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
        this.rightClickPopUp.Visible = false;
        this.currentStack = null;
        this.currentVisual = null;
        this.visualSetCalls.Clear();
    }
}