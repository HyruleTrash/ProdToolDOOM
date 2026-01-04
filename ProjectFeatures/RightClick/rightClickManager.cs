using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Button = Gum.Forms.Controls.Button;

namespace ProdToolDOOM.ProjectFeatures;

public class rightClickManager
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
        rightClickPopUp = new ContainerRuntime
        {
            IgnoredByParentSize = true,
            Visible = false
        };
        rightClickPopUp.AddToRoot();
    }
    
    public bool AddOptions<T>(RightClickOption[] options)
    {
        if (registry.ContainsKey(typeof(T)))
            return false;

        var entry = new RightClickStack(options)
        {
            rightClickOptionStack = new StackPanel()
        };

        foreach (var option in options)
        {
            option.button = new Button
            {
                Text = option.text,
            };
            option.button.Click += (_, _) => option.toCall.Invoke();
            UIParams.SetDefaultButton(option.button);
            entry.rightClickOptionStack.AddChild(option.button);
        }
        
        registry.Add(typeof(T), entry);
        return true;
    }

    public void ShowOptions<T>(Vector2 position, object selection, int priority)
    {
        visualSetCalls.Add(new RightClickVisualSetCall(typeof(T), position, selection, priority));
    }
    
    public void Update(MouseState mouseState)
    {
        ShowHighestPriority();
        if (!rightClickPopUp.Visible || !selectionBox.IsInsideBounds(new Vector2(mouseState.X, mouseState.Y)))
            Reset();
    }

    private void ShowHighestPriority()
    {
        if (currentVisual != null) return;
        currentVisual = GetHighestPriority();
        if (currentVisual == null) return;
        
        if (currentStack != null) currentStack.rightClickOptionStack.Visual.Parent = null;
        if (!registry.TryGetValue(currentVisual.Value.type, out var stack))
            return;
            
        rightClickPopUp.X = currentVisual.Value.position.x;
        rightClickPopUp.Y = currentVisual.Value.position.y;
        rightClickPopUp.Visible = true;
        rightClickPopUp.AddChild(stack.rightClickOptionStack);
        currentStack = stack;

        var size = new Vector2(rightClickPopUp.GetAbsoluteWidth(), rightClickPopUp.GetAbsoluteHeight());
        var offset = new Vector2(UIParams.minNearSelection, UIParams.minNearSelection);
        selectionBox = new SelectionBox(currentVisual.Value.position + (size / 2), size + offset);
    }

    private RightClickVisualSetCall? GetHighestPriority()
    {
        RightClickVisualSetCall? highestPriority = null;
        foreach (var setCall in visualSetCalls)
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
        rightClickPopUp.Visible = false;
        currentStack = null;
        currentVisual = null;
        visualSetCalls.Clear();
    }
}