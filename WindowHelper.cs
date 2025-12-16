using System.Runtime.InteropServices;

namespace ProdToolDOOM;

public static class WindowHelper
{
#if WINDOWS
    [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_MinimizeWindow(IntPtr window);
    [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_MaximizeWindow(IntPtr window);
    [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_RestoreWindow(IntPtr window);
    
    public static void Minimize(IntPtr window) => SDL_MinimizeWindow(window);
    public static void Maximize(IntPtr window) => SDL_MaximizeWindow(window);
    public static void UnMaximize(IntPtr window) => SDL_RestoreWindow(window);
#endif
}