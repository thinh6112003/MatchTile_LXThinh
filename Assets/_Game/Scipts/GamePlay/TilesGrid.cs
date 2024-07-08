using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public class TilesGrid : MonoBehaviour
{
    [SerializeField] private float unexposureValue = 0.85f;
    [SerializeField] private Tile tilePrefab;
    private int countLayer = 0 ;
    private float exposureValue = 1f;
    private Vector3 origin1 = new Vector3(-11.590f, 13.810f, 0);
    private Vector3 origin2 = new Vector3(-12.20f, 14.420f, 0);
    private Vector3 tileDistance = new Vector3(1.22f, -1.22f, 0);
    private Transform myTransform;
    private List<bool[,]> map = new List<bool[,]>();
    private List<List<GameObject>> listOflistTileOfLayer = new List<List<GameObject>>();
    private List<Vector3> offsetCheckExposed = new List<Vector3>{
        new Vector3(0,0,0),
        new Vector3(0.5f,0.5f,0),
        new Vector3(0.5f,-0.5f,0),
        new Vector3(-0.5f,0.5f,0),
        new Vector3(-0.5f,-0.5f,0),
    };
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
    internal void LoadMap()
    {
        string mapdata = File.ReadAllText(Application.dataPath + "/_Game/MapData/Map");
        string[] layers = mapdata.Split(";");
        InitMapData(layers);
        LoadTiles(layers);
    }
    private void InitMapData(string[] layers)
    {
        countLayer = layers.Count();
        for (int i = 0; i < countLayer; i++)
        {
            listOflistTileOfLayer.Add(new List<GameObject>());
            map.Add(new bool[21, 21]);
        }
    }
    private void LoadTiles(string[] layers)
    {
        int layerSortingOrder = 0;
        for (int layer = 0; layer < layers.Count(); layer++)
        {
            string[] rows = layers[layer].Split(",");
            LoadLayer(layer, rows, ref layerSortingOrder);
            exposureValue += 0.15f;
        }
    }
    private void LoadLayer(int layer, string[] rows,ref int layerSortingOrder)
    {
        for (int row = 0; row < 21; row++)
        {
            string[] cols = rows[row].Split(" ");
            for (int col = 0; col < 21; col++)
            {
                map[layer][row, col] = cols[col] == "1";
                if (map[layer][row, col] == true)
                {
                    InstantiateTile(layer, row, col,ref layerSortingOrder);
                }
            }
        }
    }
    private void InstantiateTile(int layer, int row, int col,ref int layerSortingOrder)
    {
        Tile newTile = Instantiate (tilePrefab,myTransform);
        Vector3 origin = (layer % 2 == 0) ? origin2 : origin1;
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
            layer: layer
        );
    }
    internal void UpdateGrid(Vector3 tilePos)
    {
        for(int i = 0; i < offsetCheckExposed.Count; i++)
        {
            RaycastHit2D tileHit = Physics2D.Raycast(tilePos+ offsetCheckExposed[i], Vector2.zero);
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
}