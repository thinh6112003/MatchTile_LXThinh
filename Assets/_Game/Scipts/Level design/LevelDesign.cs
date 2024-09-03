using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
public class LevelDesign : MonoBehaviour
{
    public static LevelDesign Instance;
    public List<List<GameObject>> listOflistTileOfLayer = new List<List<GameObject>>();
    public List<bool[,]> map = new List<bool[,]>();
    [SerializeField] private GameObject gridUnitPrefab;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileContainer;
    [SerializeField] private GameObject oddGridParent;
    [SerializeField] private GameObject evenGridParent;
    [SerializeField] private TMP_InputField layerCountInput;
    [SerializeField] private TMP_InputField oddEvenInput;
    [SerializeField] private TextMeshProUGUI layerText;
    [SerializeField] private Vector3 origin1 = new Vector3(-11.590f, 13.810f, 0);
    [SerializeField] private Vector3 origin2 = new Vector3(-12.20f, 14.420f, 0);
    [SerializeField] int state = 0;
    private int layer = 0;
    private int countLayer = -1;
    private bool parity = false;    // false = odd, true = even

    bool isMouseHold = false;
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        checkMouseHold();
        switch (state)
        {
            case 1:
                {
                    handleStatePen();
                    break;
                }
            case 2:
                {
                    handleStateEraser();
                    break;
                }
        }
    }
    void handleStatePen()
    {
        if (isMouseHold)
        {
            Debug.Log("is mouse hold on pen");
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitGrid = Physics2D.Raycast(mousePos, Vector2.zero, 0, 1 << 6);
            if (hitGrid.collider != null)
            {
                Debug.Log("is hit");
                GridUnit gridUnit = hitGrid.collider.gameObject.GetComponent<GridUnit>();
                if (map[layer][gridUnit.GetX, gridUnit.GetY] == false)
                {
                    map[layer][gridUnit.GetX, gridUnit.GetY] = true;
                    GameObject t = Instantiate
                    (
                        tilePrefab,
                        new Vector3(),
                        Quaternion.identity,
                        tileContainer.transform
                    );
                    t.transform.position = hitGrid.transform.position;
                    t.GetComponent<SpriteRenderer>().sortingOrder = layer;
                    listOflistTileOfLayer[layer].Add(t);
                }
            }
        }
    }
    void handleStateEraser()
    {
        if (isMouseHold)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitGrid = Physics2D.Raycast(mousePos, Vector2.zero, 0, 1 << 6);
            RaycastHit2D hitTile = Physics2D.Raycast(mousePos, Vector2.zero, 0, 1 << 3);
            if (hitTile.collider != null)
            {
                GridUnit gridUnit = hitGrid.collider.gameObject.GetComponent<GridUnit>();
                map[layer][gridUnit.GetX, gridUnit.GetY] = true;
                Destroy(hitTile.collider.gameObject);
            }
        }
    }
    void checkMouseHold()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            isMouseHold = true;
        }
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            isMouseHold = false;
        }
    }
    public void setLevelDesign()
    {
        countLayer = int.Parse(layerCountInput.text);
        parity = oddEvenInput.text == "odd" ? false : true;

        initGrid(true);
        // khởi tạo lưới chẵn
        initGrid(false);
        // khởi tạo lưỡi lẻ
        initMapData();

        evenGridParent.SetActive(parity);
        oddGridParent.SetActive(!parity);
    }
    public void initGrid(bool parity)
    {
        int count = parity ? 20 : 21;
        Vector3 origin = parity ? origin1 : origin2;
        Transform parent = (parity ? evenGridParent : oddGridParent).transform;
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                GameObject t = Instantiate(gridUnitPrefab, new Vector3(), Quaternion.identity, parent);
                t.transform.localPosition = origin + new Vector3(i * 1.22f, j * -1.22f, 0);
                t.GetComponent<GridUnit>().setPos(i, j);
            }
        }

    }
    public void initMapData()
    {
        for (int i = 0; i < countLayer; i++)
        {
            listOflistTileOfLayer.Add(new List<GameObject>());
            map.Add(new bool[21, 21]);
        }
    }
    public void instantiateGridUnit(int i, int j, Parity p)
    {
        GameObject t = Instantiate
        (
            gridUnitPrefab,
            new Vector3(),
            Quaternion.identity,
            p == Parity.ODD ? oddGridParent.transform : evenGridParent.transform
        );
        t.transform.localPosition = (p == Parity.ODD ? origin2 : origin1) + new Vector3(i * 1.22f, j * -1.22f, 0);
        t.GetComponent<GridUnit>().setPos(i, j);
    }
    public void changeLayer(int dolech)
    {
        if (dolech < 0) setActiveLayer(false);
        else setOpacityLayer(0.5f);

        layer += dolech;
        if (layer < 0)
            layer = 0;
        if (layer == countLayer)
            layer = countLayer - 1;

        layerText.text = layer.ToString();
        setActiveLayer(true);
        setOpacityLayer(1f);

        evenGridParent.SetActive(parity == (layer % 2 == 0));
        oddGridParent.SetActive(parity != (layer % 2 == 0));
    }
    public void setActiveLayer(bool status)
    {
        for (int i = 0; i < listOflistTileOfLayer[layer].Count; i++)
        {
            listOflistTileOfLayer[layer][i].SetActive(status);
        }
    }
    public void setOpacityLayer(float opacity)
    {
        for (int i = 0; i < listOflistTileOfLayer[layer].Count; i++)
        {
            listOflistTileOfLayer[layer][i].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, opacity);
        }
    }
    public void SaveMap()
    {
        string mapdata = "";
        for (int l = 0; l < countLayer; l++)
        {
            for (int i = 0; i < 21; i++)
            {
                for (int j = 0; j < 21; j++)
                {
                    mapdata += map[l][i, j] ? "1" : "0";
                    if (j != 20)
                        mapdata += " ";
                }
                if (i != 20)
                    mapdata += ",";
            }
            if (l != countLayer - 1)
                mapdata += ";";
        }
        mapdata = (parity ? "even" : "odd") + "|" + mapdata;
        File.WriteAllText(Application.dataPath + "/_Game/MapData/Map", mapdata);
    }
    public void changeState(int newState)
    {
        Debug.Log("is change state");
        state = newState;
    }
}
public enum Parity
{
    ODD, // lẻ 
    EVEN // chẵn
}