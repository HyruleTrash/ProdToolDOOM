
#if WINDOWS
using System.Windows.Forms;
#endif

namespace ProdToolDOOM;

public static class FileExplorerHelper
{
    public interface IFileDialogService
    {
        string OpenFile(string initialDirectory = "c:\\");
    }

    public class DesktopFileDialogService : IFileDialogService
    {
        public string OpenFile(string initialDirectory = "c:\\")
        {
            #if WINDOWS
            using var openFileDialog = new OpenFileDialog();
            
            openFileDialog.InitialDirectory = initialDirectory;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK) 
                return openFileDialog.FileName;
            
            #elif MACOS || LINUX
            // Use platform-specific implementation
            // return FileDialog.OpenFile(filter);
            #endif
            return null;
        }
    }

    private static IFileDialogService fileDialogService;

    [STAThread]
    public static string OpenFileExplorer()
    {
        if (fileDialogService == null)
        {
            #if WINDOWS || MACOS || LINUX
            fileDialogService = new DesktopFileDialogService();
            #endif
        }
        if (fileDialogService != null)
            return fileDialogService.OpenFile();
        else
            return "";
    }
}