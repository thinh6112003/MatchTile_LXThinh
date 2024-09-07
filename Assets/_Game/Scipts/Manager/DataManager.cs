using UnityEngine;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public LevelSO levelSO;
    public SpriteSO spriteSO;
    public int coin = 100;
    public static DynamicData dynamicData;
    private void Awake()
    {
        GetPlayerpref();
        if (dynamicData == null)
        {
            dynamicData = new DynamicData();
            dynamicData.currentLevel = 1;
            dynamicData.coin = 10;
        }
        coin = dynamicData.coin;
        Instance = this;
    }
    private void Start()
    {
        Observer.AddListener(Notifi.VICTORY_GAME, VictoryUpdateData);
    }
    private void VictoryUpdateData()
    {
        dynamicData.currentLevel++;
    }
    public void GetPlayerpref()
    {
        string dynamicDataString = PlayerPrefs.GetString("dynamicData");
        dynamicData = JsonUtility.FromJson<DynamicData>(dynamicDataString);
    }
    public void SetPlayerpref()
    {
        dynamicData.coin = coin;
        string dynamicDataString = JsonUtility.ToJson(dynamicData);
        PlayerPrefs.SetString("dynamicData", dynamicDataString);
        //PlayerPrefs.DeleteKey("dynamicData");  // reset playerpref
    }
    private void OnApplicationQuit()
    {
        SetPlayerpref();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SetPlayerpref();
        }
    }

}
public class DynamicData
{
    public int currentLevel;
    public int coin;
}
public class Notifi
{
    public static string PAUSE_GAME = "Pause";
    public static string DEFEAT_GAME = "Defeat";
    public static string VICTORY_GAME = "Victory";
}





