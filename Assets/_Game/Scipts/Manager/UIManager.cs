using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mapLevelScene;
    [SerializeField] private GameObject gamePlayScene;
    [SerializeField] private GameObject homeScene;
    [SerializeField] private GameObject levelDesignScene;
    [SerializeField] private GameObject victoryPopup;
    [SerializeField] private GameObject defeatPopup;
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private GameObject settingPopup;
    [SerializeField] private GameObject congratsPopup;
    [SerializeField] private GameObject blinds;
    public static UIManager Instance;
    private GameObject currentScene;
    private GameObject beforeScene;
    private GameObject currentPopup;
    private void Start()
    {
        Instance = this;
        navigateToScene("Home");
    }
    public void navigateToScene(string nameScene)
    {
        beforeScene = currentScene;
        if (currentScene != null)
        {
            currentScene.SetActive(false);
        }
        if (currentPopup != null)
        {
            currentPopup.SetActive(false);
        }
        switch (nameScene)
        {
            case "Home":
                homeScene.SetActive(true);
                currentScene = homeScene;
                break;
            case "MapLevel":
                mapLevelScene.SetActive(true);
                currentScene = mapLevelScene;
                break;
            case "LevelDesign":
                levelDesignScene.SetActive(true);
                currentScene = levelDesignScene;
                break;
            case "GamePlay":
                gamePlayScene.SetActive(true);
                currentScene = gamePlayScene;
                break;
        }
    }
    public void navigateToBeforeScene()
    {
        if (currentScene != null)
        {
            if (beforeScene == gamePlayScene)
            {
                Observer.Noti("BackToGame");
            }
            currentScene.SetActive(false);
            GameObject temp = currentScene;
            currentScene = beforeScene;
            currentScene.SetActive(true);
            beforeScene = temp;
        }
        else
        {
            Debug.Log("error before scene null!!!");
        }
    }
    public void loadPopup(string namePopup)
    {
        switch (namePopup)
        {
            case "Victory":
                victoryPopup.SetActive(true);
                currentPopup = victoryPopup;
                break;
            case "Defeat":
                defeatPopup.SetActive(true);
                currentPopup = defeatPopup;
                break;
            case "Pause":
                pausePopup.SetActive(true);
                currentPopup = pausePopup;
                break;
            case "Setting":
                settingPopup.SetActive(true);
                currentPopup = settingPopup;
                break;
            case "Congrats":
                congratsPopup.SetActive(true);
                currentPopup = congratsPopup;
                break;
        }
    }
    public void hidePopup()
    {
        currentPopup.SetActive(false);
        currentPopup = null;
    }
}
