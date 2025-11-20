
#if WINDOWS
using System.Windows.Forms;
#endif

namespace ProdToolDOOM;

public static class FileExplorerHelper
{
    public interface IFileDialogService
    {
        string OpenFile(string initialDirectory = "c:\\");
        string SaveFile(string initialDirectory = "c:\\");
    }

    public class DesktopFileDialogService : IFileDialogService
    {
        public string OpenFile(string initialDirectory = "c:\\")
        {
            #if WINDOWS
            using var openFileDialog = new OpenFileDialog();
            
            openFileDialog.InitialDirectory = initialDirectory;
            openFileDialog.Filter = "xml files (*.xml)|*.xml";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK) 
                return openFileDialog.FileName;
            
            #elif MACOS || LINUX
            // Use platform-specific implementation
            // return FileDialog.OpenFile(filter);
            #endif
            return null;
        }

        public string SaveFile(string initialDirectory = "c:\\")
        {
            #if WINDOWS
            using var saveFileDialog = new SaveFileDialog();
            
            saveFileDialog.InitialDirectory = initialDirectory;
            saveFileDialog.Filter = "xml files (*.xml)|*.xml";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK) 
                return saveFileDialog.FileName;
            #endif
            return null;
        }
    }

    private static IFileDialogService fileDialogService;

    [STAThread]
    public static string OpenFileExplorer(string path = "c:\\")
    {
        if (HasFileExplorer())
            return fileDialogService.OpenFile(path);
        else
            return String.Empty;
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
    public static string SaveWithFileExplorer(string path = "c:\\")
    {
        if (HasFileExplorer())
            return fileDialogService.SaveFile(path);
        else
            return String.Empty;
    }
}