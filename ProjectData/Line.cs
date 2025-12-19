namespace ProdToolDOOM;

public class Line
{
    public int Id { get => id; set => id = value; }
    public int IdOther { get => idOther; set => idOther = value; }
    private int id;
    private int idOther;

    public Line(int id, int idOther)
    {
        this.id = id;
        this.idOther = idOther;
    }
    public Line(Line line)
    {
        id = line.Id;
        idOther = line.IdOther;
    }
    public Line(){}
}