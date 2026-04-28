namespace ProdToolDOOM;

public class EntityData
{
    public string ImagePath { get => this.imagePath; set => this.imagePath = value; }
    public string Name { get => this.name; set => this.name = value; }
    private string imagePath = "No image path";
    private string name = "Please fill in a name";
    private List<Entity> registeredEntities = [];

    public EntityData() { }
    public EntityData(EntityData other)
    {
        this.name = other.Name;
        this.imagePath = other.ImagePath;
    }
    
    public void AddEntityRegistration(Entity entity) => this.registeredEntities.Add(entity);
    public void RemoveEntityRegistration(Entity entity) => this.registeredEntities.Remove(entity);

    public void SetEntityRegistration(int i)
    {
        foreach (Entity entity in this.registeredEntities)
            entity.DataId = i;
    }
}