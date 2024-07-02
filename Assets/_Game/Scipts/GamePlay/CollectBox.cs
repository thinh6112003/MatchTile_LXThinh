using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class CollectBox : MonoBehaviour
{
    [SerializeField] private Vector3 firstTilePos= new Vector3(0.65f,0.2f,0f);
    [SerializeField] private Vector3 tilesDistance = new Vector3(1.455f,0f,0f);
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private int idTileInsert= 0;
    [SerializeField] private bool isNotAutoInsert= false;
    [SerializeField] private int maxCollectTile= 8 ;
    private Transform myTransform;
    private int dem = 0;
    private List<Tile> listCurrentTile = new List<Tile>();
    private List<Tile> listMatchTile = new List<Tile>();
    private void Start()
    {
        myTransform = this.transform;
        Test();
    }
    private void Test()
    {
        if(dem<= 100)
        {
            Tile newTile =  Instantiate(tilePrefab,myTransform);
            newTile.transform.localPosition = new Vector3(3,10,0);
            if (isNotAutoInsert)
                newTile.setTile(idTileInsert);
            else 
                newTile.setTile();
            StartCoroutine(AddTile(newTile));
            Invoke(nameof(Test), 0.5f);
        }
        dem++;
    }
    private IEnumerator AddTile(Tile newTile)
    {
        if (listCurrentTile.Count == maxCollectTile) yield return null;
        yield return new WaitForSeconds(0.5f);
        int insertIndex= listCurrentTile.Count;
        InsertTile(newTile, ref insertIndex);
        yield return new WaitForSeconds(0.8f);
        if (CheckThreeMatch(insertIndex, newTile))
        {
            HideMatchTile(insertIndex);
        }
        yield return new WaitForSeconds(0.25f);
        if ( CheckThreeMatch(insertIndex, newTile) ) {
            MoveUpTiles(insertIndex);

        }
        yield return null;
    }
    private int InsertTile(Tile tile, ref int insertIndex)
    {
        for (int i = listCurrentTile.Count - 1; i >= 0; i--)
        {
            if (listCurrentTile[i].getSpriteID() == tile.getSpriteID())
            {
                insertIndex = i + 1;
                break;
            }
        }
        listCurrentTile.Add(null);
        if (insertIndex != listCurrentTile.Count - 1)
        {
            for (int i = listCurrentTile.Count - 2; i >= insertIndex; i--)
            {
                listCurrentTile[i + 1] = listCurrentTile[i];
                listCurrentTile[i].transform.DOMove(listCurrentTile[i].transform.position + tilesDistance, 0.5f);
            }
        }
        tile.transform.DOMove(myTransform.TransformPoint(firstTilePos + insertIndex * tilesDistance), 0.75f);
        listCurrentTile[insertIndex] = tile;
        return insertIndex;
    }
    private bool CheckThreeMatch(int insertIndex, Tile newTile)
    {

        if (insertIndex >= 2)
        {
            int newTileSpriteID = newTile.getSpriteID();
            if (listCurrentTile[insertIndex - 1].getSpriteID() != newTileSpriteID ||
                listCurrentTile[insertIndex - 2].getSpriteID() != newTileSpriteID)
            {
                Debug.Log("three is false");
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }
    private void HideMatchTile(int newMatchIndex)
    {
        for (int i = newMatchIndex; i >= newMatchIndex - 2; i--)
        {
            listCurrentTile[i].transform.DOScale(new Vector3(0,0), 0.2f);
            listMatchTile.Add(listCurrentTile[i]);
        }
    }
    private void RemoveMatchTile(int newMatchIndex)
    {
        for (int i = 0; i >= 2; i++)
        {
            Destroy(listMatchTile[i]);
            // sau này sẽ nâng cấp thành pooling
        }
    }
    private void MoveUpTiles(int newMatchIndex)
    {
        for (int i = newMatchIndex + 1; i < listCurrentTile.Count; i++)
        {
            listCurrentTile[i].transform.DOMove(listCurrentTile[i].transform.position - tilesDistance*3, 0.5f);
            listCurrentTile[i - 3] = listCurrentTile[i];
        }
        int lastIndex = listCurrentTile.Count-1;
        for (int i = lastIndex; i >= lastIndex-2; i--)
        {
            listCurrentTile.RemoveAt(i);
        }
    }
}
