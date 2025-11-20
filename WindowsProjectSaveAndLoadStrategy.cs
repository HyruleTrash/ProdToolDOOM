namespace ProdToolDOOM;

public class WindowsProjectSaveAndLoadStrategy : IProjectSaveAndLoadStrategy
{
    public bool Load(string path)
    {
        using var openFileDialog = new OpenFileDialog();
        openFileDialog.FileName = path;
        try
        {
            var fileStream = openFileDialog.OpenFile();

            using var reader = new StreamReader(fileStream);
            var fileContent = reader.ReadToEnd();
            Debug.Log(fileContent);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    public bool Save(string path)
    {
        throw new NotImplementedException();
    }
}