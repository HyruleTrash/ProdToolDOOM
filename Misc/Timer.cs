namespace ProdToolDOOM;

public class Timer
{
    private double currentTime;
    private readonly float maxTime;
    public Action? onEnd;
    public Action<double>? onPlaying;
    public bool running;

    public Timer(float maxTime, Action onEnd)
    {
        this.maxTime = maxTime;
        this.onEnd = onEnd;
        running = true;
    }
        
    public Timer(float maxTime)
    {
        this.maxTime = maxTime;
        running = true;
    }
        
    public void Reset()
    {
        currentTime = 0;
        running = true;
    }

    public void Update(double dt)
    {
        if (!running)
            return;
        currentTime += dt;
        onPlaying?.Invoke(currentTime);
        CheckIfEndIsReached();
    }

    private void CheckIfEndIsReached()
    {
        if (!(currentTime >= maxTime)) return;
        onEnd?.Invoke();
        running = false;
    }

    public double GetCurrentTime()
    {
        return currentTime;
    }
        
    private static string GetFormattedTime(double time)
    {
        var timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
        
    public string GetFormattedTime(bool countDown = false)
    {
        return countDown ? GetFormattedTime(maxTime - currentTime) : GetFormattedTime(currentTime);
    }
}