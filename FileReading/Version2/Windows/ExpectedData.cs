
#if WINDOWS
using System.Xml;
namespace ProdToolDOOM.Version2;

public class ExpectedData
{
    public string name;
    public string? stopAt = null;
    public bool found;
    public Action<XmlReader> load;
}
    
public interface IExpectedCollectionData
{
    public abstract void loadEntry(XmlReader reader);
    public abstract void saveEntry();
}
#endif