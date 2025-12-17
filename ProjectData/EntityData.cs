namespace ProdToolDOOM;

public class EntityData
{
    public string ImagePath { get => imagePath; set => imagePath = value; }
    public string Name { get => name; set => name = value; }
    private string imagePath = "No image path";
    private string name = "Please fill in a name";

    public EntityData() { }
    public EntityData(EntityData other)
    {
        name = other.Name;
        imagePath = other.ImagePath;
    }
}