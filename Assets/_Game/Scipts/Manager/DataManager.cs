using UnityEngine;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public SpriteSO spriteSO;
    private void Start()
    {
        Instance = this;
    }
}
public class Notifi
{
    public static string END_GAME = "end game";
}