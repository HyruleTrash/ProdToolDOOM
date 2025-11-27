using System.IO.Compression;
using System.Xml;

namespace ProdToolDOOM.Version1;

public class WindowsProjectLoadStrategy : IProjectLoadStrategy
{
    private static List<ExpectedData> expectedData = new ();
    
    public WindowsProjectLoadStrategy()
    {
        expectedData.Add(new ExpectedData() { name = "Project_Version", load = (XmlReader reader) =>
        {
            string version = reader.ReadElementContentAsString();
            Debug.Log($"File project version: {version}");
        } });
        expectedData.Add(new ExpectedLevelData(this));
        expectedData.Add(new ExpectedEntityData(this)); 
    }
    
    public bool Load(string path)
    {
        if (path == string.Empty || path == null)
            return false;
        try
        {
            using var archive = ZipFile.OpenRead(path);
            foreach (var entry in archive.Entries)
            {
                if (entry.Name != "projectData.xml") continue;
                using var reader = XmlReader.Create(entry.Open());
                Project.ResetData();
                ReadData(reader, new(expectedData));
                
                // TODO remove this
                Debug.Log("Levels:");
                foreach (var level in Project.levels)
                {
                    Debug.Log(" Level:");
                    Debug.Log("  Entities:");
                    foreach (var entity in level.Entities)
                    {
                        Debug.Log("   Entity:");
                        Debug.Log($"   id: {entity.Id}");
                        Debug.Log($"   x: {entity.XPosition}, y: {entity.YPosition}");
                    }
                }
                Debug.Log("EntityData:");
                foreach (var data in Project.entityDatas)
                {
                    Debug.Log(" EntityData:");
                    Debug.Log($"  Id: {data.Key}");
                    Debug.Log($"  Name: {data.Value.Name}, ImagePath: {data.Value.ImagePath}");
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

    private struct CollectionData
    {
        public string? collectionName = null;
        public string? collectionType = null;
        public int collectionCount = 0;
        public int collectionIndex = 0;

        public CollectionData() { }
    }
    
    public void ReadData(XmlReader reader, List<ExpectedData> searchData, string? stopAt = null)
    {
        CollectionData collectionData = new CollectionData();
        
        while (reader.Read())
        {
            if (reader is not { NodeType: XmlNodeType.Element })
                continue;
            if (stopAt != null && reader.NodeType == XmlNodeType.EndElement && reader.Name == stopAt)
                break;
            
            foreach (var dataInstance in searchData)
            {
                if (dataInstance.name != reader.Name)
                    continue;
                if (dataInstance is IExpectedCollectionData expectedCollectionData)
                {
                    CheckCollection(reader, collectionData, expectedCollectionData);
                }
                else
                {
                    dataInstance.load.Invoke(reader);
                }

                dataInstance.found = true;
            }
            
            if (searchData.All(data => data.found))
                break;
        }
    }

    /// <summary>
    /// Calls logic if XML element is a desired collection
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="collectionData"></param>
    /// <param name="dataInstance"></param>
    private void CheckCollection(XmlReader reader, CollectionData collectionData, IExpectedCollectionData dataInstance)
    {
        if (!IsCollection(reader, collectionData, out collectionData))
            return;
                    
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.EndElement && !reader.Name.Contains("Entry") &&
                reader.Name.Contains(collectionData.collectionName))
            {
                Debug.Log($"Collection read finished (endEntry encountered {reader.Name})");
                break;
            }
                        
            if (reader is not { NodeType: XmlNodeType.Element })
                return;
                        
            var foundEntry = ReadCollectionEntry(reader, collectionData, dataInstance);
            if (foundEntry && collectionData.collectionIndex == collectionData.collectionCount)
            {
                Debug.Log("Collection read finished (count)");
                break;
            }
        }
    }
    
    /// <summary>
    /// Checks if the current Xml element is a collection, if so outputs the correct collection data
    /// </summary>
    /// <param name="reader">For checking the XML data</param>
    /// <param name="collectionData">current collection data, so that it may be updated</param>
    /// <param name="outCollectionData">found collection data</param>
    /// <returns></returns>
    private static bool IsCollection(XmlReader reader, CollectionData collectionData, out CollectionData outCollectionData)
    {
        outCollectionData = collectionData;
        if (reader is { HasAttributes: true })
        {
            outCollectionData.collectionType = reader.GetAttribute("collectionType");
            if (outCollectionData.collectionType is "List" or "Dictionary")
            {
                outCollectionData.collectionName = reader.Name;
                Int32.TryParse(reader.GetAttribute("count"), out outCollectionData.collectionCount);
                return true;
            }
        }
        
        outCollectionData = new CollectionData();
        return false;
    }

    private bool ReadCollectionEntry(XmlReader reader, CollectionData collectionData, IExpectedCollectionData data)
    {
        if (collectionData.collectionName == null || 
            !reader.Name.Contains("Entry") || 
            !reader.Name.Contains(collectionData.collectionName)
        )
        {
            return false;
        }

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Contains("Entry") &&
                reader.Name.Contains(collectionData.collectionName))
            {
                collectionData.collectionIndex++;
                data.saveEntry();
                return true;
            }
            
            data.loadEntry(reader);
        }

        return true;
    }
}