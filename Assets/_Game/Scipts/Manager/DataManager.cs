using UnityEngine;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public SpriteSO spriteSO;
    public static DynamicData dynamicData = new DynamicData();
    private void Awake()
    {
        Instance = this;
        dynamicData.currentLevel = 1;
    }
    private void Start()
    {
        Observer.AddListener(Notifi.VICTORY_GAME, VictoryUpdateData);
    }
    private void VictoryUpdateData()
    {
        dynamicData.currentLevel++;
    }
}
public class DynamicData
{
    public int currentLevel;
}
public class Notifi
{
    public static string PAUSE_GAME = "Pause";
    public static string DEFEAT_GAME = "Defeat";
    public static string VICTORY_GAME = "Victory";
}





