namespace ProdToolDOOM;

public interface IBaseUpdatable
{
    /// <param name="dt">Delta time</param>
    public void Update(float dt, WindowInstance windowRef);
}