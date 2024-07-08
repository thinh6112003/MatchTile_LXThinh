using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Intance;
    [SerializeField] private CollectBox collectBox;
    [SerializeField] private TilesGrid tileGrid;  
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
    }
    internal void SelectTileListener()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D tileHit = Physics2D.Raycast(mousePos, Vector2.zero, 0, 1<<7);

            if (tileHit.collider == null) return;

            Tile newTileAdd = tileHit.collider.gameObject.GetComponent<Tile>();
            newTileAdd.DeactiveCollider();
            tileGrid.UpdateGrid(newTileAdd.transform.position);
            collectBox.AddTile(newTileAdd);
        }
    }

}
