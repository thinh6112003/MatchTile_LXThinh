using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Intance;
    [SerializeField] private CollectBox collectBox;
    [SerializeField] private TilesGrid tileGrid;
    private Tile tileSelect = null;
    private bool isEndGame = false;
    private void Awake()
    {
        //target frame rate ve 60 fps
        Application.targetFrameRate = 60;
        Intance = this;
        isEndGame = false;
    }
    private void Start()
    {
        InitGameLevel();
    }
    private void Update()
    {
        if(!isEndGame) SelectTileListener();
    }
    private void InitGameLevel()
    {
        Observer.AddListener (Notifi.END_GAME, HandleEndGame);
        tileGrid.LoadMap();
    }
    private void HandleEndGame()
    {
        isEndGame = true;
        Invoke("HideBox", 0.6f);
    }

    private void HideBox()
    {
        collectBox.HideBox();
    }
    internal void SelectTileListener()
    {
        if((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))&&tileSelect == null)
        {
            tileSelect = getTileSelect();
            if (tileSelect == null) return;
            tileSelect.selectTile();
        }
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))&& tileSelect != null)
        {
            Tile newTileAdd = getTileSelect();
            if (newTileAdd != tileSelect.GetComponent<Tile>())
            {
                tileSelect.GetComponent<Tile>().deSelectTile();
                tileSelect = null;
                return;
            }
            newTileAdd.DeactiveCollider();
            tileGrid.UpdateGrid(newTileAdd.transform.position);
            collectBox.AddTile(newTileAdd);
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
