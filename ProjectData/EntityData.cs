namespace ProdToolDOOM;

public class EntityData
{
    public string ImagePath { get => imagePath; set => imagePath = value; }
    public string Name { get => name; set => name = value; }
    private string imagePath = "No image path";
    private string name = "Please fill in a name";
    private List<Entity> registeredEntities = [];

    public EntityData() { }
    public EntityData(EntityData other)
    {
        name = other.Name;
        imagePath = other.ImagePath;
    }
    
    public void AddEntityRegistration(Entity entity) => registeredEntities.Add(entity);
    public void RemoveEntityRegistration(Entity entity) => registeredEntities.Remove(entity);

    public void SetEntityRegistration(int i)
    {
        foreach (var entity in registeredEntities)
            entity.Id = i;
    }
}