using System.Runtime.InteropServices;

namespace ProdToolDOOM.Window;

public static class Helper
{
#if WINDOWS
    [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_MinimizeWindow(IntPtr window);
    [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_MaximizeWindow(IntPtr window);
    [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_RestoreWindow(IntPtr window);
    [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint SDL_GetWindowFlags(IntPtr window);
    
    public static void Minimize(IntPtr window) => SDL_MinimizeWindow(window);
    public static void Maximize(IntPtr window) => SDL_MaximizeWindow(window);
    public static void UnMaximize(IntPtr window) => SDL_RestoreWindow(window);
    
    public static bool HasFocus(IntPtr windowHandle)
    {
        var flags = SDL_GetWindowFlags(windowHandle);
        return (flags & 0x00000200) != 0;
    }

#endif
}