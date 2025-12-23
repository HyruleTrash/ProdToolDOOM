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
    private RightClickStack? currentStack;
    public object? currentSelection;

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

    public void ShowOptions<T>(Vector2 position)
    {
        if (currentStack != null) currentStack.rightClickOptionStack.Visual.Parent = null;
        if (!registry.TryGetValue(typeof(T), out var stack))
            return;
        
        rightClickPopUp.X = position.x;
        rightClickPopUp.Y = position.y;
        rightClickPopUp.Visible = true;
        rightClickPopUp.AddChild(stack.rightClickOptionStack);
        currentStack = stack;

        var size = new Vector2(rightClickPopUp.GetAbsoluteWidth(), rightClickPopUp.GetAbsoluteHeight());
        var offset = new Vector2(UIParams.minNearSelection, UIParams.minNearSelection);
        selectionBox = new SelectionBox(position + (size / 2), size + offset);
    }
    
    public void Update(MouseState getState)
    {
        if (!rightClickPopUp.Visible || selectionBox.IsInsideBounds(new Vector2(getState.X, getState.Y))) return;
        Reset();
    }
    
    public void Reset()
    {
        rightClickPopUp.Visible = false;
        currentStack = null;
        currentSelection = null;
    }
}