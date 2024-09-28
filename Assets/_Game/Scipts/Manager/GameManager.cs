using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Intance;     // singleton

    [SerializeField] private CollectBox collectBox;
    [SerializeField] private TilesGrid tileGrid;
    [SerializeField] private GameObject background;
    private Tile tileSelect = null;
    private bool isEndGame = false;
    private bool isPlay = false;
    private int currentLevel = 1;
    private Sprite originBackground;
    private void Awake()
    {
        Intance = this;    // singleton 
        //target frame rate ve 60 fps
        Application.targetFrameRate = 60;
        isEndGame = false;
        originBackground = background.GetComponent<Image>().sprite;
    }
    private void Start()
    {
        Observer.AddListener("BackToGame", ActiveGamePlay);
        Observer.AddListener(Notifi.PAUSE_GAME, PauseGame);
        tileGrid.gameObject.SetActive(false);
        Observer.AddListener(Notifi.DEFEAT_GAME, HandleDefeat);
        Observer.AddListener(Notifi.DEFEAT_GAME, ClockManager.instance.stopClock);
        Observer.AddListener(Notifi.VICTORY_GAME, HandleVictory);
        Observer.AddListener(Notifi.VICTORY_GAME, ClockManager.instance.stopClock);
    }
    private void Update()
    {
        if (!isEndGame && isPlay) SelectTileListener();
    }
    public void ActiveGamePlay()
    {
        isPlay = true;
        tileGrid.gameObject.SetActive(true);
        collectBox.gameObject.SetActive(true);
        background.GetComponent<Image>().sprite = DataManager.Instance.levelSO.listLevel[currentLevel - 1].background;
        UIManager.Instance.setCoin();
    }
    public void DeActiveGamePlay()
    {
        isPlay = false;
        tileGrid.gameObject.SetActive(false);
        collectBox.gameObject.SetActive(false);
        background.GetComponent<Image>().sprite = originBackground;
    }
    public void InitGameLevel(int iDLevel = -1)
    {   // iDlevel = 0 Init level tiếp theo
        // iDlevel = - 1 Init level hiện tại
        // iDlevel = -2 Init level mới chơi(khi bấm chơi lại)
        // iDlevel > 0 Init level duoc chon
        ClockManager.instance.startClock();
        if (iDLevel == -1 && DataManager.dynamicData.currentLevel == 11)
        {
            // TH đã chơi hết các màn 
            UIManager.Instance.navigateToScene("Home");
            UIManager.Instance.loadPopup("Congrats");
            DeActiveGamePlay();
            return;
        }
        isPlay = true;
        isEndGame = false;

        tileGrid.gameObject.SetActive(true);
        collectBox.gameObject.SetActive(true);
        collectBox.Init();

        if (iDLevel == 0 && DataManager.dynamicData.currentLevel == 11) DataManager.dynamicData.currentLevel = 10;
        if (iDLevel == -1 || iDLevel == 0) currentLevel = DataManager.dynamicData.currentLevel;
        else if (iDLevel != -2) currentLevel = iDLevel;

        background.GetComponent<Image>().sprite = DataManager.Instance.levelSO.listLevel[currentLevel - 1].background;
        DataManager.Instance.spriteSO = DataManager.Instance.levelSO.listLevel[currentLevel - 1].tileSet;

        tileGrid.maxTile = DataManager.Instance.levelSO.listLevel[currentLevel - 1].numberTypes;
        tileGrid.LoadMap(currentLevel);

        UIManager.Instance.setLevelText(currentLevel);
    }
    public void PauseGame()
    {
        isPlay = false;
    }
    public void ResumeGame()
    {
        isPlay = true;
    }
    private void HandleDefeat()
    {
        isEndGame = true;
        Invoke("HideBox", 0.6f);
        Invoke("ShowDefeat", 1.2f);
    }
    private void HideBox()
    {
        collectBox.HideBox();
    }
    private void ShowDefeat()
    {
        UIManager.Instance.loadPopup("Defeat");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameDefeat);
    }
    private void HandleVictory()
    {
        Debug.Log("victory");
        Invoke("ShowVictory", 1.5f);
    }
    private void ShowVictory()
    {
        UIManager.Instance.loadPopup("Victory");
        UIManager.Instance.setVictory(10, ClockManager.time);
        DataManager.Instance.coin += 10;
        Debug.Log("tang coin");
        UIManager.Instance.setCoin();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameVictory);
    }
    internal void SelectTileListener()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && tileSelect == null)
        {
            tileSelect = getTileSelect();
            if (tileSelect == null) return;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.tapTile);
            tileSelect.selectTile();
        }
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && tileSelect != null)
        {
            Tile newTileAdd = getTileSelect();
            if (newTileAdd != tileSelect.GetComponent<Tile>())
            {
                tileSelect.GetComponent<Tile>().deSelectTile();
                tileSelect = null;
                return;
            }
            newTileAdd.DeactiveCollider();
            tileGrid.UpdateGrid(newTileAdd.gameObject);
            collectBox.AddTile(newTileAdd);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.fly);
            tileSelect = null;
        }
    }
    private Tile getTileSelect()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D tileHit = Physics2D.Raycast(mousePos, Vector2.zero, 0, 1 << 7);
        if (tileHit.collider == null) return null;
        return tileHit.collider.gameObject.GetComponent<Tile>();
    }
    // neu game co nhieu lop chong nhau, de trong 3d hon thi gan thay doi rotation cua tileGrid -> (1.5;1;0)
}
