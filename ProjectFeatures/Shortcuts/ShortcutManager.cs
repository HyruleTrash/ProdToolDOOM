using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ProdToolDOOM;

public class ShortcutManager : IBaseUpdatable
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
        this.shortCuts = [];
        this.pressTimer = new Timer(1, () => this.ableToUseShortCuts = true)
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
        if (this.shortCuts.TryGetValue(shortCut.priority, out List<ShortCut>? existingShortcuts))
            existingShortcuts.Add(shortCut);
        else
            this.shortCuts.Add(shortCut.priority, [shortCut]);
    }

    public void AddShortCuts(ShortCut[] shortcuts)
    {
        foreach (ShortCut t in shortcuts)
            AddShortCut(t);
    }

    public void RemoveShortCut(string shortCutName)
    {
        foreach (KeyValuePair<int, List<ShortCut>> shortCutList in this.shortCuts)
        {
            bool shouldStop = false;
            foreach (ShortCut shortCut in shortCutList.Value)
            {
                if (shortCut.keyCombinationString != shortCutName) continue;
                shortCutList.Value.Remove(shortCut);
                if (shortCutList.Value.Count == 0) this.shortCuts.Remove(shortCutList.Key);
                shouldStop = true;
                break;
            }
            if (shouldStop)
                break;
        }
    }
    
    private static string GenerateKeyCombinationString(ShortCut shortCut)
    {
        string result = "";
        for (int i = 0; i < shortCut.keyCombination.Length; i++)
        {
            Keys key = shortCut.keyCombination[i];
            result += $"{key}";
            if (i != shortCut.keyCombination.Length - 1)
                result += "+";
        }
        return result;
    }
    
    private static int CalculatePriority(ShortCut shortCut) => shortCut.keyCombination.Length;

    public void Update(float dt, WindowInstance windowRef)
    {
        KeyboardState keyboardState = windowRef.KeyboardState;
        if (!this.ableToUseShortCuts)
        {
            this.pressTimer.Update(dt);
            if (this.lastPressed != null && !CheckShortcut(keyboardState, this.lastPressed))
            {
                this.ableToUseShortCuts = true;
                this.lastPressed = null;
            }
            return;
        }

        if (!CheckShortcuts(keyboardState)) return;
        this.ableToUseShortCuts = false;
        this.pressTimer.Reset();
    }

    private bool CheckShortcuts(KeyboardState keyboardState)
    {
        if (this.shortCuts.Count == 0) return false;
        
        bool shortcutFound = false;
        for (int index = this.shortCuts.Count - 1; index >= 0; index--)
        {
            List<ShortCut> shortCutList = this.shortCuts.Values[index];
            
            foreach (ShortCut shortCut in shortCutList)
            {
                if (!CheckShortcut(keyboardState, shortCut)) continue;
                shortcutFound = true;
                this.lastPressed = shortCut;
                break;
            }

            if (shortcutFound)
                break;
        }
        
        return shortcutFound;
    }

    private bool CheckShortcut(KeyboardState keyboardState, ShortCut shortCut)
    {
        bool isBeingPressed = true;
        foreach (Keys key in shortCut.keyCombination)
        {
            if (keyboardState.IsKeyDown(key)) continue;
            isBeingPressed = false;
            break;
        }

        if (!isBeingPressed) return false;

        if (this.lastPressed != shortCut || !this.pressTimer.running)
        {
            Debug.Log($"Calling {shortCut.keyCombinationString}");
            shortCut.action.Invoke();
        }

        return true;
    }
}