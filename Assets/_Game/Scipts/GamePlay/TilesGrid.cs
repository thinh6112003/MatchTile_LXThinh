using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class TilesGrid : MonoBehaviour
{
    [SerializeField] private float unexposureValue = 0.5f;
    [SerializeField] private Tile tilePrefab;
    public int maxTile = 24;
    private int countLayer = 0;
    private float exposureValue = 1f;
    private Vector3 origin1 = new Vector3(-11.590f, 13.810f, 0);// chuan roi
    private Vector3 origin2 = new Vector3(-12.20f, 14.420f, 0);
    private Vector3 tileDistance = new Vector3(1.22f, -1.22f, 0);
    private Transform myTransform;
    private List<bool[,]> map = new List<bool[,]>();
    private List<GameObject> listTileOfTileGrid = new List<GameObject>();   // //////
    private List<Vector3> offsetCheckExposed = new List<Vector3>{
        new Vector3(0,0,0),
        new Vector3(0.5f,0.5f,0),
        new Vector3(0.5f,-0.5f,0),
        new Vector3(-0.5f,0.5f,0),
        new Vector3(-0.5f,-0.5f,0),
    };
    private List<int> listTileID = new List<int>();
    private bool parity = false;    // false = odd, true = even
    int countTiles = 0;
    private void Awake()
    {
        myTransform = this.gameObject.transform;
    }
    private void Start()
    {
        for (int i = 0; i < offsetCheckExposed.Count; i++)
        {
            offsetCheckExposed[i] *= myTransform.localScale.x;
        }

    }
    private void InitTileGrid()
    {
        listTileOfTileGrid = new List<GameObject>();
        map = new List<bool[,]>();
        listTileID = new List<int>();
        countTiles = 0;
    }
    internal void LoadMap(int iDmap)
    {
        ListRemoveAll();
        InitTileGrid();
        string path = "Map" + iDmap.ToString();
        string mapdataString = Resources.Load<TextAsset>(path).text;
        caculateCountType(mapdataString);
        RandomGridTiles();
        string[] mapdata = mapdataString.Split("|");
        parity = mapdata[0] == "odd" ? false : true;
        string[] layers = mapdata[1].Split(";");
        InitMapData(layers);
        LoadTiles(layers);
    }
    private void ListRemoveAll()
    {
        for (int i = 0; i < listTileOfTileGrid.Count; i++)
        {
            Destroy(listTileOfTileGrid[i]);
        }
    }
    private void RandomGridTiles()
    {
        // init idtile list to match 3
        for (int i = 0; i < countTiles / 3; i++)
        {
            int newID = Random.Range(0, maxTile);
            listTileID.Add(newID);
            listTileID.Add(newID);
            listTileID.Add(newID);
        }
        // mix idtile list
        for (int i = 0; i < countTiles; i++)
        {
            int MixIndex = Random.Range(0, countTiles);
            int term = listTileID[i];
            listTileID[i] = listTileID[MixIndex];
            listTileID[MixIndex] = term;
        }
    }
    private void caculateCountType(string stringData)
    {
        for (int i = 0; i < stringData.Length; i++)
        {
            if (stringData[i] == '1') countTiles++;
        }
    }
    private void InitMapData(string[] layers)
    {
        countLayer = layers.Count();
        for (int i = 0; i < countLayer; i++)
        {
            map.Add(new bool[21, 21]);
        }
    }
    private void LoadTiles(string[] layers)
    {
        int currentIndexTile = 0;
        int layerSortingOrder = 0;
        for (int layer = 0; layer < layers.Count(); layer++)
        {
            string[] rows = layers[layer].Split(",");
            LoadLayer(layer, rows, ref layerSortingOrder, ref currentIndexTile);
            exposureValue += 0.15f;
        }
    }
    private void LoadLayer(int layer, string[] rows, ref int layerSortingOrder, ref int currentIndexTile)
    {
        for (int row = 0; row < 21; row++)
        {
            string[] cols = rows[row].Split(" ");
            for (int col = 0; col < 21; col++)
            {
                map[layer][row, col] = cols[col] == "1";
                if (map[layer][row, col] == true)
                {
                    InstantiateTile(layer, row, col, ref layerSortingOrder, ref currentIndexTile);
                    currentIndexTile++;
                }
            }
        }
    }
    private void InstantiateTile(int layer, int row, int col, ref int layerSortingOrder, ref int currentIndexTile)
    {
        Tile newTile = Instantiate(tilePrefab, myTransform);
        listTileOfTileGrid.Add(newTile.gameObject);
        Vector3 origin = (parity ? !(layer % 2 == 0) : (layer % 2 == 0)) ? origin2 : origin1;
        float tileColorValue = unexposureValue;
        LayerMask layerMark = LayerMask.NameToLayer("Unexposed");
        if (layer == countLayer - 1)
        {
            tileColorValue = exposureValue;
            layerMark = LayerMask.NameToLayer("Exposed");
        }

        newTile.setTile(
            localPos: origin + new Vector3(row * tileDistance.x, col * tileDistance.y, -layer),
            color: new Vector4(tileColorValue, tileColorValue, tileColorValue, 1f),
            sortingOder: ref layerSortingOrder,
            layerMark: layerMark,
            layer: layer,
            idTile: listTileID[currentIndexTile]

        );
    }
    internal void UpdateGrid(GameObject selectedTile)
    {
        Vector3 tilePos = selectedTile.transform.position;
        listTileOfTileGrid.Remove(selectedTile);
        if (listTileOfTileGrid.Count == 0) Observer.Noti("Victory");      //check win game
        for (int i = 0; i < offsetCheckExposed.Count; i++)
        {
            RaycastHit2D tileHit = Physics2D.Raycast(tilePos + offsetCheckExposed[i], Vector2.zero);
            if (tileHit.collider == null) continue;
            Tile tileBehind = tileHit.collider.gameObject.GetComponent<Tile>();
            if (CheckExposed(tileBehind.transform.position, tileBehind.getGridLayer()))
            {
                tileHit.collider.gameObject.GetComponent<Tile>().SetExposed();
            }
        }
    }
    private bool CheckExposed(Vector3 tilePos, int tileGridLayer)
    {
        for (int j = 0; j < offsetCheckExposed.Count; j++)
        {
            RaycastHit2D tileHitCheck = Physics2D.Raycast(tilePos + offsetCheckExposed[j], Vector2.zero);
            if (tileHitCheck.collider != null)
            {
                if (tileHitCheck.collider.gameObject.GetComponent<Tile>().getGridLayer() > tileGridLayer)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void HoanDoi()
    {
        if (DataManager.Instance.coin < 3)
        {
            UIManager.Instance.showNoti();
            return;
        }
        DataManager.Instance.coin -= 3;
        UIManager.Instance.setCoin();
        for (int i = 0; i < listTileOfTileGrid.Count; i++)
        {
            int MixIndex = Random.Range(0, listTileOfTileGrid.Count);
            Tile tile1 = listTileOfTileGrid[i].GetComponent<Tile>();
            Tile tile2 = listTileOfTileGrid[MixIndex].GetComponent<Tile>();
            int id = tile1.spriteID;               //
            tile1.spriteID = tile2.spriteID;  //
            tile2.spriteID = id;      //
            Sprite sprite = tile1.mySpriteRenderer.sprite;
            Sprite sprite2 = tile1.spriteRenderer.sprite;
            tile1.mySpriteRenderer.sprite = tile2.mySpriteRenderer.sprite;
            tile1.spriteRenderer.sprite = tile2.spriteRenderer.sprite;
            tile2.spriteRenderer.sprite = sprite2;
            tile2.mySpriteRenderer.sprite = sprite;
        }
    }
}
