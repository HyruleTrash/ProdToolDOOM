using System.IO.Compression;

#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version2;

public class ProjectLoadStrategy : IProjectLoadStrategy
{
    private List<ExpectedData> expectedData = [];
    private bool shouldQuit = false;
    private string? foundVersion;
    
    public ProjectLoadStrategy()
    {
        SetExpectedData();
    }

    private void SetExpectedData()
    {
        this.expectedData.Clear();
        this.expectedData.Add(new ExpectedData { name = "Project_Version", load = reader =>
        {
            string version = reader.ReadElementContentAsString();
            switch (version)
            {
                case "0.0.2":
                    Debug.Log($"File project version: {version}");
                    break;
                case "0.0.1":
                    this.shouldQuit = true;
                    this.foundVersion = version;
                    return;
            }
        } });
        this.expectedData.Add(new ExpectedData { name = "Id_Counter", load = reader =>
        {
            int counter = reader.ReadElementContentAsInt();
            Project.instance.entityDataIdCounter = counter;
        }});
        this.expectedData.Add(new ExpectedEntityData(this));
        this.expectedData.Add(new ExpectedLevelData(this));
        this.expectedData.Add(new ExpectedData { name = "Current_Level", load = reader =>
        {
            int levelId = reader.ReadElementContentAsInt();
            Project.instance.CurrentLevel = levelId;
        }});
    }
    
    public bool Load(string path)
    {
        this.shouldQuit = false;
        this.foundVersion = null;
        SetExpectedData();
        
        if (path == string.Empty || path == null)
            return false;
        try
        {
            using ZipArchive archive = ZipFile.OpenRead(path);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.Name != "projectData.xml") continue;
                using XmlReader reader = XmlReader.Create(entry.Open());
                Project.instance.ResetData();
                ReadData(reader, new(this.expectedData));

                if (this.shouldQuit)
                {
                    Debug.Log($"old version encountered {this.foundVersion}");
                    return VersionManager.LoadUsingOldStrategy(this.foundVersion, path);
                }
                
                if (Project.instance.CurrentLevel < 0 || Project.instance.CurrentLevel >= Project.instance.levels.Count)
                    return true;
                
                foreach (Line line in Project.instance.levels[Project.instance.CurrentLevel].Lines) line.Init();
                foreach (Point point in Project.instance.levels[Project.instance.CurrentLevel].Points) point.Init();
                foreach (Entity entity in Project.instance.levels[Project.instance.CurrentLevel].Entities) entity.Init();
                
                // TODO remove this
                Debug.Log($"Levels: {Project.instance.levels.Count}");
                Debug.Log($"Current level: {Project.instance.CurrentLevel}");
                foreach (Level level in Project.instance.levels)
                {
                    Debug.Log(" Level:");
                    Debug.Log("  Entities:");
                    foreach (Entity entity in level.Entities)
                    {
                        Debug.Log("   Entity:");
                        Debug.Log($"    id: {entity.DataId}");
                        Debug.Log($"    position: {entity.Position}");
                    }
                    Debug.Log("  Points:");
                    foreach (Point point in level.Points)
                    {
                        Debug.Log($"   Point: {point}");
                    }
                    Debug.Log("  Lines:");
                    foreach (Line line in level.Lines)
                    {
                        Debug.Log($"   Line: {line.Id} to {line.IdOther}");
                    }
                }
                Debug.Log($"EntityData: {Project.instance.entityDatas.Count}");
                foreach (KeyValuePair<int, EntityData> data in Project.instance.entityDatas)
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
            Debug.LogError(e);
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
        CollectionData collectionData = new();
        
        while (true)
        {
            foreach (ExpectedData dataInstance in searchData)
            {
                if (this.shouldQuit)
                    return;
                
                if (dataInstance.stopAt != null && reader.NodeType == XmlNodeType.EndElement &&
                    reader.Name == dataInstance.stopAt)
                {
                    dataInstance.found = true;
                    Debug.Log("Exiting due to stop at: " + reader.Name);
                    continue;
                }
                
                if (dataInstance.name != reader.Name)
                    continue;
                if (dataInstance is IExpectedCollectionData expectedCollectionData)
                    CheckCollection(reader, collectionData, expectedCollectionData);
                else
                    dataInstance.load.Invoke(reader);

                dataInstance.found = true;
            }
            
            if (searchData.All(data => data.found))
                break;
            foreach (ExpectedData dataInstance in searchData) Debug.Log($"Status: {dataInstance.name}, {dataInstance.stopAt}, {dataInstance.found}");

            reader.Read();
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
        
        if (collectionData.collectionCount == 0)
        {
            Debug.Log($"Skipping empty collection: {collectionData.collectionName}");
            return;
        }
                    
        while (true)
        {
            if (this.shouldQuit)
                return;
            if (reader.NodeType == XmlNodeType.EndElement && !reader.Name.Contains("Entry") &&
                reader.Name.Contains(collectionData.collectionName))
            {
                Debug.Log($"Collection read finished (endEntry encountered {reader.Name}, {collectionData.collectionName})");
                break;
            }
                        
            bool foundEntry = ReadCollectionEntry(reader, collectionData, dataInstance);
            if (foundEntry && collectionData.collectionIndex == collectionData.collectionCount)
            {
                Debug.Log($"Collection read finished (count) {collectionData.collectionName}");
                break;
            }

            reader.Read();
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

        while (true)
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

            reader.Read();
        }

        return true;
    }
}
#endif