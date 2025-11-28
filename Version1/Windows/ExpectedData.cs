using System.Xml;

#if WINDOWS
namespace ProdToolDOOM.Version1;

public class ExpectedData
{
    public string name;
    public bool found;
    public Action<XmlReader> load;
}
    
public interface IExpectedCollectionData
{
    public abstract void loadEntry(XmlReader reader);
    public abstract void saveEntry();
}
#endif