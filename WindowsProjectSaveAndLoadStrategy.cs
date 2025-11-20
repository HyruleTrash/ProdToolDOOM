using System.Xml;

namespace ProdToolDOOM;

public class WindowsProjectSaveAndLoadStrategy : IProjectSaveAndLoadStrategy
{
    public bool Load(string path)
    {
        if (path == String.Empty)
            return false;
        try
        {
            using (var reader = XmlReader.Create(path))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Project_Version")
                    {
                        string version = reader.ReadElementContentAsString();
                        Debug.Log($"Project_Version: {version}");
                    }
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

    public bool Save(string path)
    {
        if (path == String.Empty || path == null)
            return false;
        
        using (var writer = XmlWriter.Create(path))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("root");
            
            writer.WriteStartElement("Project_Version");
            writer.WriteString(Program.PROGRAM_VERSION);
            writer.WriteEndElement();
            
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
        
        return true;
    }
}