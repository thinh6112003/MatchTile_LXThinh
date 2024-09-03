using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Intance;
    [SerializeField] private CollectBox collectBox;
    [SerializeField] private TilesGrid tileGrid;
    private Tile tileSelect = null;
    private bool isEndGame = false;
    private bool isPlay = false;
    private int currentLevel = 1;
    private void Awake()
    {
        //target frame rate ve 60 fps
        Application.targetFrameRate = 60;
        Intance = this;
        isEndGame = false;
    }
    private void Start()
    {
        Observer.AddListener("BackToGame", ActiveGamePlay);
        Observer.AddListener(Notifi.PAUSE_GAME, PauseGame);
        tileGrid.gameObject.SetActive(false);
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
    }
    public void DeActiveGamePlay()
    {
        isPlay = false;
        tileGrid.gameObject.SetActive(false);
        collectBox.gameObject.SetActive(false);
    }
    public void InitGameLevel(int iDLevel = -1)
    {
        if (iDLevel == -1 && DataManager.dynamicData.currentLevel == 11)
        {
            UIManager.Instance.navigateToScene("Home");
            UIManager.Instance.loadPopup("Congrats");
            DeActiveGamePlay();
            return;
        }
        if (iDLevel == 0 && DataManager.dynamicData.currentLevel == 11) DataManager.dynamicData.currentLevel = 10;
        collectBox.Init();
        isPlay = true;
        isEndGame = false;
        Observer.AddListener(Notifi.DEFEAT_GAME, HandleDefeat);
        Observer.AddListener(Notifi.VICTORY_GAME, HandleVictory);
        if (iDLevel == -1 || iDLevel == 0) currentLevel = DataManager.dynamicData.currentLevel;
        else if (iDLevel != -2) currentLevel = iDLevel;
        tileGrid.LoadMap(currentLevel);
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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameLose);
    }
    private void HandleVictory()
    {
        Invoke("ShowVictory", 1.5f);
    }
    private void ShowVictory()
    {
        UIManager.Instance.loadPopup("Victory");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameWin);
    }
    internal void SelectTileListener()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && tileSelect == null)
        {
            tileSelect = getTileSelect();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.tapTile);
            if (tileSelect == null) return;
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
