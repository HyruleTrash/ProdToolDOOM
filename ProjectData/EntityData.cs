namespace ProdToolDOOM;

public class EntityData
{
    public string ImagePath { get; set; }
    public string Name { get; set; }
    private readonly List<Entity> registeredEntities = [];

    public EntityData(string name = "Please fill in a name", string imagePath = "No image path")
    {
        this.Name = name;
        this.ImagePath = imagePath;
    }
    
    public EntityData(EntityData other)
    {
        this.Name = other.Name;
        this.ImagePath = other.ImagePath;
    }
    
    public void AddEntityRegistration(Entity entity) => this.registeredEntities.Add(entity);
    public void RemoveEntityRegistration(Entity entity) => this.registeredEntities.Remove(entity);

    public void SetEntityRegistration(int i)
    {
        foreach (Entity entity in this.registeredEntities)
            entity.DataId = i;
    }
}