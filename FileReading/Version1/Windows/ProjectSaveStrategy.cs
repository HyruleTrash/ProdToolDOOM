using System.IO;
using System.IO.Compression;

#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version1;

public class ProjectSaveStrategy : IProjectSaveStrategy
{
    public bool Save(string path)
    {
        if (path == string.Empty)
            return false;

        string usedPath = path;
        bool exists = File.Exists(path);
        if (exists)
        {
            usedPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + $"tempSave{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.wapd";
        }
        
        try
        {
            using (ZipArchive archive = ZipFile.Open(usedPath, ZipArchiveMode.Create))
            {
                ZipArchiveEntry entry = archive.CreateEntry("projectData.xml");
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
            Debug.LogError(e.Message);
            return false;
        }
    }

    private void WriteProjectData(Stream output)
    {
        using XmlWriter writer = XmlWriter.Create(output);
        writer.WriteStartDocument();
        writer.WriteStartElement("root");
            
        writer.WriteStartElement("Project_Version");
        writer.WriteString(Program.instance.PROGRAM_VERSION);
        writer.WriteEndElement();
        
        new ReflectionSerializer<Level, XmlWriter>().SerializeList(Project.instance.levels, "Levels", writer);
        
        writer.WriteStartElement("Id_Counter");
        writer.WriteString(Project.instance.entityDataIdCounter.ToString());
        writer.WriteEndElement();
        
        new ReflectionSerializer<EntityData, XmlWriter>().SerializeDictionary(new Dictionary<int, EntityData>(Project.instance.EntityDatas), "EntityData", writer);
            
        writer.WriteEndElement();
        writer.WriteEndDocument();
    }
}
#endif