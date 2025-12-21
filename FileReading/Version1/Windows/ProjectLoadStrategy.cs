using System.IO.Compression;

#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version1;

public class ProjectLoadStrategy : IProjectLoadStrategy
{
    private static List<ExpectedData> expectedData = new ();
    
    public ProjectLoadStrategy()
    {
        expectedData.Add(new ExpectedData() { name = "Project_Version", load = (XmlReader reader) =>
        {
            string version = reader.ReadElementContentAsString();
            Debug.Log($"File project version: {version}");
        } });
        expectedData.Add(new ExpectedLevelData(this));
        expectedData.Add(new ExpectedData() { name = "Id_Counter", load = (XmlReader reader) =>
        {
            int counter = reader.ReadElementContentAsInt();
            Project.instance.idCounter = counter;
        }});
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
                Project.instance.ResetData();
                ReadData(reader, new(expectedData));
                
                // TODO remove this
                Debug.Log($"Levels: {Project.instance.levels.Count}");
                foreach (var level in Project.instance.levels)
                {
                    Debug.Log(" Level:");
                    Debug.Log("  Entities:");
                    foreach (var entity in level.Entities)
                    {
                        Debug.Log("   Entity:");
                        Debug.Log($"    id: {entity.Id}");
                        Debug.Log($"    position: {entity.Position}");
                    }
                    Debug.Log("  Points:");
                    foreach (var point in level.Points)
                    {
                        Debug.Log($"   Point: {point}");
                    }
                    Debug.Log("  Lines:");
                    foreach (var line in level.Lines)
                    {
                        Debug.Log($"   Line: {line.Id} to {line.IdOther}");
                    }
                }
                Debug.Log($"EntityData: {Project.instance.entityDatas.Count}");
                foreach (var data in Project.instance.entityDatas)
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
            Debug.LogError(e.Message);
            return false;
        }
    }

    private class CollectionData
    {
        public string? collectionName = null;
        public string? collectionType = null;
        public int collectionCount = 0;
        public int collectionIndex = 0;

        public CollectionData() { }
    }
    
    public void ReadData(XmlReader reader, List<ExpectedData> searchData)
    {
        CollectionData collectionData = new CollectionData();
        
        while (reader.Read())
        {
            foreach (var dataInstance in searchData)
            {
                if (dataInstance.stopAt != null && reader.NodeType == XmlNodeType.EndElement &&
                    reader.Name == dataInstance.stopAt)
                {
                    dataInstance.found = true;
                    continue;
                }
                
                if (dataInstance.name != reader.Name || reader.NodeType != XmlNodeType.Element)
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
                Debug.Log($"Collection read finished (endEntry encountered {reader.Name}, {collectionData.collectionName})");
                break;
            }
                        
            var foundEntry = ReadCollectionEntry(reader, collectionData, dataInstance);
            if (foundEntry && collectionData.collectionIndex == collectionData.collectionCount)
            {
                Debug.Log($"Collection read finished (count) {collectionData.collectionName}");
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
                outCollectionData.collectionIndex = 0;
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
        
        int entryDepth = reader.Depth;
        bool hasEntered = false;

        while (reader.Read())
        {
            if (reader.Depth > entryDepth)
                hasEntered = true;
            
            data.loadEntry(reader);
            
            if ((reader.NodeType == XmlNodeType.EndElement || (hasEntered && entryDepth == reader.Depth)) && 
                reader.Name.Contains("Entry") && reader.Name.Contains(collectionData.collectionName))
            {
                collectionData.collectionIndex++;
                data.saveEntry();
                return true;
            }
        }

        return true;
    }
}
#endif