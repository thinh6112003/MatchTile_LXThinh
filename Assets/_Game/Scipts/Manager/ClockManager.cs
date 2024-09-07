using UnityEngine;

public class ClockManager : MonoBehaviour
{
    public static ClockManager instance;
    public static int time = 0;
    bool play = true;
    private void Start()
    {
        instance = this;
    }
    public void startClock()
    {
        time = 0;
        play = true;
        addtime();
    }
    public void stopClock()
    {
        play = false;
    }
    public void addtime()
    {
        if (play == false) return;
        time++;
        Invoke(nameof(addtime), 1);
    }
}
