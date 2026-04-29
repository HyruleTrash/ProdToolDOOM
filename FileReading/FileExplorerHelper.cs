

using System.Runtime.CompilerServices;
#if WINDOWS
using System.Windows.Forms;
#endif

namespace ProdToolDOOM;

public static class FileExplorerHelper
{
    public struct FileDialogResult
    {
        public string filePath;
        public string fileExtension;
    }

    public interface IFileDialogService
    {
        string CheckForDefaultDir(string? initialDirectory);
        FileDialogResult? OpenFile(string? initialDirectory);
        FileDialogResult? SaveFile(string filter, string? initialDirectory);
    }

    public class DesktopFileDialogService : IFileDialogService
    {
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

            if (openFileDialog.ShowDialog() == DialogResult.OK)
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

            if (saveFileDialog.ShowDialog() == DialogResult.OK) 
                return new FileDialogResult 
                    { filePath = saveFileDialog.FileName, fileExtension = Path.GetExtension(saveFileDialog.FileName) };
            #endif
            return null;
        }
    }

    private static IFileDialogService fileDialogService;

    [STAThread]
    public static FileDialogResult? OpenFileExplorer(string? path = null)
    {
        if (HasFileExplorer())
            return fileDialogService.OpenFile(path);
        else
            return null;
    }

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