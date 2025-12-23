using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ProdToolDOOM;

public class ShortcutManager
{
    public class ShortCut(Keys[] keyCombination, Action action)
    {
        public readonly Keys[] keyCombination = keyCombination;
        public readonly Action action = action;
        public string keyCombinationString = "";
        public int priority = -1;
    }
    
    private SortedList<int, List<ShortCut>> shortCuts = [];
    private Timer pressTimer;
    private bool ableToUseShortCuts = true;
    private ShortCut? lastPressed;

    public ShortcutManager()
    {
        shortCuts = [];
        pressTimer = new Timer(1, () => ableToUseShortCuts = true)
        {
            running = false
        };
    }

    public ShortcutManager(ShortCut[] shortCuts) : this()
    {
        AddShortCuts(shortCuts);
    }

    public void AddShortCut(ShortCut shortCut)
    {
        shortCut.keyCombinationString = GenerateKeyCombinationString(shortCut);
        shortCut.priority = CalculatePriority(shortCut);
        if (shortCuts.TryGetValue(shortCut.priority, out List<ShortCut>? existingShortcuts))
            existingShortcuts.Add(shortCut);
        else
            shortCuts.Add(shortCut.priority, [shortCut]);
    }

    public void AddShortCuts(ShortCut[] shortcuts)
    {
        foreach (var t in shortcuts)
            AddShortCut(t);
    }

    public void RemoveShortCut(string shortCutName)
    {
        foreach (var shortCutList in shortCuts)
        {
            var shouldStop = false;
            foreach (var shortCut in shortCutList.Value)
            {
                if (shortCut.keyCombinationString != shortCutName) continue;
                shortCutList.Value.Remove(shortCut);
                if (shortCutList.Value.Count == 0)
                    shortCuts.Remove(shortCutList.Key);
                shouldStop = true;
                break;
            }
            if (shouldStop)
                break;
        }
    }
    
    private static string GenerateKeyCombinationString(ShortCut shortCut)
    {
        var result = "";
        for (var i = 0; i < shortCut.keyCombination.Length; i++)
        {
            var key = shortCut.keyCombination[i];
            result += $"{key}";
            if (i != shortCut.keyCombination.Length - 1)
                result += "+";
        }
        return result;
    }
    
    private static int CalculatePriority(ShortCut shortCut) => shortCut.keyCombination.Length;

    public void Update(KeyboardState keyboardState, float dt)
    {
        if (!ableToUseShortCuts)
        {
            pressTimer.Update(dt);
            if (lastPressed != null && !CheckShortcut(keyboardState, lastPressed))
            {
                ableToUseShortCuts = true;
                lastPressed = null;
            }
            return;
        }

        if (!CheckShortcuts(keyboardState)) return;
        ableToUseShortCuts = false;
        pressTimer.Reset();
    }

    private bool CheckShortcuts(KeyboardState keyboardState)
    {
        if (shortCuts.Count == 0) return false;
        
        var shortcutFound = false;
        for (var index = shortCuts.Count - 1; index >= 0; index--)
        {
            var shortCutList = shortCuts.Values[index];
            
            foreach (var shortCut in shortCutList)
            {
                if (!CheckShortcut(keyboardState, shortCut)) continue;
                shortcutFound = true;
                lastPressed = shortCut;
                break;
            }

            if (shortcutFound)
                break;
        }
        
        return shortcutFound;
    }

    private bool CheckShortcut(KeyboardState keyboardState, ShortCut shortCut)
    {
        var isBeingPressed = true;
        foreach (var key in shortCut.keyCombination)
        {
            if (keyboardState.IsKeyDown(key)) continue;
            isBeingPressed = false;
            break;
        }

        if (!isBeingPressed) return false;

        if (lastPressed != shortCut || !pressTimer.running)
        {
            Debug.Log($"Calling {shortCut.keyCombinationString}");
            shortCut.action.Invoke();
        }

        return true;
    }
}