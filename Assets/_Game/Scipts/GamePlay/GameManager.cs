using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public List<List<GameObject>> listOflistTileOfLayer = new List<List<GameObject>>();
    public List<bool[,]> map = new List<bool[,]>();
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileContainer;
    Vector3 origin1 = new Vector3(-11.590f, 13.810f, 0);
    Vector3 origin2 = new Vector3(-12.20f, 14.420f, 0);
    public static GameManager Intance;
    private void Awake()
    {
        //target frame rate ve 60 fps
        Application.targetFrameRate = 60;
        Intance = this;
        LoadMap();
        Observer.AddListener
        (
            Notifi.END_GAME, 
            () =>
            {
                Debug.Log("end game roi !!!!!!!!!!!!!!");
            }
        );
    }
    public void LoadMap()
    {

        string mapdata = File.ReadAllText(Application.dataPath + "/_Game/MapData/Map");
        string[] layers = mapdata.Split(";");
        for (int i = 0; i < layers.Count(); i++)
        {
            listOflistTileOfLayer.Add(new List<GameObject>());
            map.Add(new bool[21, 21]);
        }
        int currentSortingOrder = 0;
        float exposureValue = 1f - 0.15f * (2);
        for (int l = 0; l < layers.Count(); l++)
        {
            string[] rows = layers[l].Split(",");
            for (int i = 0; i < 21; i++)
            {
                string[] cols = rows[i].Split(" ");
                for (int j = 0; j < 21; j++)
                {
                    map[l][i, j] = cols[j] == "1" ? true : false;
                    if (map[l][i, j] == true)
                    {
                        GameObject newTile = Instantiate
                        (
                            tilePrefab,
                            new Vector3(),
                            Quaternion.identity,
                            tileContainer.transform
                        );
                        Vector3 origin = l % 2 == 0 ? origin2 : origin1;
                        newTile.transform.localPosition = origin + new Vector3(i * 1.22f, j * -1.22f, 0);
                        SpriteRenderer newTileSR = newTile.GetComponent<SpriteRenderer>();
                        newTileSR.sortingOrder = currentSortingOrder++;
                        newTileSR.color = new Color(exposureValue, exposureValue, exposureValue, 1f);
                    }
                }
            }
            exposureValue += 0.15f;
        }
    }
}
