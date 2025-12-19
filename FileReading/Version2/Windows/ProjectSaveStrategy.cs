using System.IO;
using System.IO.Compression;

#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version2;

public class ProjectSaveStrategy : IProjectSaveStrategy
{
    public bool Save(string path)
    {
        if (path == string.Empty)
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
        writer.WriteString(Program.instance.PROGRAM_VERSION);
        writer.WriteEndElement();
        writer.WriteStartElement("Id_Counter");
        writer.WriteString(Project.instance.idCounter.ToString());
        writer.WriteEndElement();
        
        new ReflectionSerializer<EntityData, XmlWriter>().SerializeDictionary(Project.instance.entityDatas, "EntityData", writer);
        new ReflectionSerializer<Level, XmlWriter>().SerializeList(Project.instance.levels, "Levels", writer);
        
        writer.WriteEndElement();
        writer.WriteEndDocument();
    }
}
#endif