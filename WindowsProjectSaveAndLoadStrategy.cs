using System.Xml;
using System.IO;
using System.IO.Compression;

namespace ProdToolDOOM;

public class WindowsProjectSaveAndLoadStrategy : IProjectSaveAndLoadStrategy
{
    public bool Load(string path)
    {
        if (path == String.Empty)
            return false;
        try
        {
            using var archive = ZipFile.OpenRead(path);
            foreach (var entry in archive.Entries)
            {
                if (entry.Name == "projectData.xml")
                {
                    ReadProjectData(entry.Open());
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    private void ReadProjectData(Stream fileStream)
    {
        using var reader = XmlReader.Create(fileStream);
        
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Project_Version")
            {
                string version = reader.ReadElementContentAsString();
                Debug.Log($"Project_Version: {version}");
            }
        }
    }

    public bool Save(string path)
    {
        if (path == String.Empty || path == null)
            return false;

        var usedPath = path;
        var exists = File.Exists(path);
        if (exists)
        {
            usedPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + $"tempSave{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.wapd";
        }
        
        try
        {
            using (var archive = ZipFile.Open(usedPath, ZipArchiveMode.Create))
            {
                var entry = archive.CreateEntry("projectData.xml");
                WriteProjectData(entry.Open());
            }

            if (!exists) return true;
            // replacing old file with new
            File.Delete(path);
            File.Move(usedPath, path);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    private void WriteProjectData(Stream output)
    {
        using var writer = XmlWriter.Create(output);
        writer.WriteStartDocument();
        writer.WriteStartElement("root");
            
        writer.WriteStartElement("Project_Version");
        writer.WriteString(Program.PROGRAM_VERSION);
        writer.WriteEndElement();
        
        new ReflectionSerializer<Level, XmlWriter>().SerializeList(Project.levels, "Levels", writer);
            
        writer.WriteEndElement();
        writer.WriteEndDocument();
    }
}