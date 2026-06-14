namespace DLLevelBuilder;

public class SwitchLevelCmd(Project project, int? direction = null, int? newId = null) : ICommand
{
    private int previousLevelId;
    
    public void Execute()
    {
        if (direction != null)
        {
            project.CurrentLevel += direction.Value;
            Debug.Log($"Switched level to {project.CurrentLevel}");
        }
        else if (newId != null)
        {
            this.previousLevelId = project.CurrentLevel;
            project.CurrentLevel = newId.Value;
            Debug.Log($"Switched level to {project.CurrentLevel}");
        }
    }

    public void Undo()
    {
        if (direction != null)
        {
            project.CurrentLevel -= direction.Value;
            Debug.Log($"Switched level to {project.CurrentLevel}");
        }
        else if (newId != null)
        {
            project.CurrentLevel = this.previousLevelId;
            Debug.Log($"Switched level to {project.CurrentLevel}");
        }
    }
}