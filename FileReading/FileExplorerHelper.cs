
using System;
using System.IO;
#if WINDOWS
using System.Windows.Forms;
using System.Runtime.InteropServices;
#endif

namespace DLLevelBuilder;

public static class FileExplorerHelper
{
    public struct FileDialogResult
    {
        public string filePath;
        public string fileExtension;
    }

    private interface IFileDialogService
    {
        string CheckForDefaultDir(string? initialDirectory);
        FileDialogResult? OpenFile(string? initialDirectory);
        FileDialogResult? SaveFile(string filter, string? initialDirectory);
    }

    private class DesktopFileDialogService : IFileDialogService
    {
        #if WINDOWS
        private class WindowWrapper(IntPtr handle) : IWin32Window
        {
            public IntPtr Handle { get; } = handle;
        }
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string? lpszWindow);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HwndTopmost = new(-1);
        private const uint SwpNoSize = 0x0001;
        private const uint SwpNoMove = 0x0002;
        private const uint SwpShowWindow = 0x0040;
        
        // [DllImport("user32.dll")]
        // private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        //
        // private const uint GwEnabledPopup = 6;
        #endif
        
        public string CheckForDefaultDir(string? initialDirectory) => 
            initialDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public FileDialogResult? OpenFile(string? initialDirectory)
        {
            string usedDir = CheckForDefaultDir(initialDirectory);
            #if WINDOWS
            using OpenFileDialog openFileDialog = new();
            
            openFileDialog.InitialDirectory = usedDir;
            openFileDialog.Filter = "wapd files (*.wapd)|*.wapd";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            
            IntPtr mainHandle = GetForegroundWindow();
            MakeTopWindowPosAvailable();

            if (openFileDialog.ShowDialog(new WindowWrapper(mainHandle)) == DialogResult.OK)
                return new FileDialogResult 
                    { filePath = openFileDialog.FileName, fileExtension = Path.GetExtension(openFileDialog.FileName) };
            
            #elif MACOS || LINUX
            // Use platform-specific implementation
            // return FileDialog.OpenFile(filter);
            #endif
            return null;
        }

        public FileDialogResult? SaveFile(string filter, string? initialDirectory)
        {
            string usedDir = CheckForDefaultDir(initialDirectory);
            #if WINDOWS
            using SaveFileDialog saveFileDialog = new();
            
            saveFileDialog.InitialDirectory = usedDir;
            saveFileDialog.Filter = filter;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            
            IntPtr mainHandle = GetForegroundWindow();
            MakeTopWindowPosAvailable();

            if (saveFileDialog.ShowDialog(new WindowWrapper(mainHandle)) == DialogResult.OK) 
                return new FileDialogResult 
                    { filePath = saveFileDialog.FileName, fileExtension = Path.GetExtension(saveFileDialog.FileName) };
            #endif
            return null;
        }
        
        #if WINDOWS
        private static void MakeTopWindowPosAvailable()
        {
            uint currentPid = (uint)Environment.ProcessId;

            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    IntPtr dialogHwnd = IntPtr.Zero;

                    while ((dialogHwnd = FindWindowEx(IntPtr.Zero, dialogHwnd, "#32770", null)) != IntPtr.Zero)
                    {
                        GetWindowThreadProcessId(dialogHwnd, out uint windowPid);
                        if (windowPid != currentPid) continue;
                        SetWindowPos(dialogHwnd, HwndTopmost, 0, 0, 0, 0, SwpNoMove | SwpNoSize | SwpShowWindow);
                        SetForegroundWindow(dialogHwnd);
                        return;
                    }
                    Thread.Sleep(20);
                }
            });
        }
        #endif
    }

    private static IFileDialogService fileDialogService;

    [STAThread]
    public static FileDialogResult? OpenFileExplorer(string? path = null) => HasFileExplorer() ? fileDialogService.OpenFile(path) : null;

    private static bool HasFileExplorer()
    {
        if (fileDialogService == null)
        {
            #if WINDOWS || MACOS || LINUX
            fileDialogService = new DesktopFileDialogService();
            #endif
        }

        return fileDialogService != null;
    }
    
    [STAThread]
    public static FileDialogResult? SaveWithFileExplorer(string filter, string? path = null) => 
        HasFileExplorer() ? fileDialogService.SaveFile(filter, path) : null;
}