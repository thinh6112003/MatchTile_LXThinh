using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System;
public class CollectBox : MonoBehaviour
{
    [SerializeField] private Vector3 firstTilePos= new Vector3(0.65f,0.2f,0f);
    [SerializeField] private Vector3 tilesDistance = new Vector3(1.455f,0f,0f);
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private int idTileInsert= 0;
    [SerializeField] private bool isNotAutoInsert= false;
    [SerializeField] private bool isHighSpeed= false;
    [SerializeField] private int maxCollectTile= 8 ;
    private Transform myTransform;
    private int dem = 0;
    private List<Tile> listCurrentTile = new List<Tile>();
    private int demhighspeed = 0 ;
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
            //if (isNotAutoInsert)
            //    newTile.setTile(idTileInsert);
            //else 
            //    newTile.setTile();
            StartCoroutine(AddTile(newTile));
            if (isHighSpeed)
            {
                demhighspeed++;
            }
            if (demhighspeed == 15)
            {
                isHighSpeed = false;
                demhighspeed = 0;
            }
            if (dem < 3)
            {
                newTile.setTile(0);
            }
            else
            if (dem < 6)
            {
                newTile.setTile(1);
            }
            else {
                newTile.setTile();
            }
            Invoke(nameof(Test), isHighSpeed? 0.25f :  1f);
        }
        dem++;
    }
    private IEnumerator AddTile(Tile newTile)
    {
        //if (listCurrentTile.Count == maxCollectTile) yield return null;
        yield return new WaitForSeconds(0.2f);
        int insertIndex= listCurrentTile.Count;
        MoveToInsertTile(newTile, ref insertIndex);
        if (CheckThreeMatch(insertIndex, newTile))
        {
            Dictionary<Tile, Vector3> dicTileToTarget = new Dictionary<Tile, Vector3>();
            List<Tile> listMatchTile = new List<Tile>();
            Debug.Log("is three match");
            MoveUpTilesModel(insertIndex, dicTileToTarget, listMatchTile);
            yield return new WaitForSeconds(0.7f);
            HideMatchTile(listMatchTile);
            yield return new WaitForSeconds(0.2f);
            Debug.Log("MoveUpTilesView,RemoveMatchTile");
            MoveUpTilesView(dicTileToTarget);
            RemoveMatchTile(listMatchTile);
        }
        yield return null;
    }
    private int MoveToInsertTile(Tile tile, ref int insertIndex)
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
        if (insertIndex != listCurrentTile.Count-1)
        {
            for (int i = listCurrentTile.Count - 2; i >= insertIndex; i--)
            {
                listCurrentTile[i].transform.DOMove(myTransform.TransformPoint(firstTilePos + tilesDistance*(i+1)), 0.5f);
                listCurrentTile[i + 1] = listCurrentTile[i];
            }
        }
        tile.transform.DOMove(myTransform.TransformPoint(firstTilePos + insertIndex * tilesDistance), 0.7f);
        listCurrentTile[insertIndex] = tile;
        return insertIndex;
    }
    private bool CheckThreeMatch(int insertIndex, Tile newTile)
    {
        //return false; // test dont check three match
        if (insertIndex >= 2)
        {
            int newTileSpriteID = newTile.getSpriteID();
            if (listCurrentTile[insertIndex - 1].getSpriteID() != newTileSpriteID ||
                listCurrentTile[insertIndex - 2].getSpriteID() != newTileSpriteID)
            {
                return false;
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    private void HideMatchTile(List<Tile> listMatchTile)
    {
        for (int i = 0; i <= 2; i++)
        {
            listMatchTile[i].gameObject.transform.DOScale(new Vector3(0, 0), 0.2f);
        }
    }
    private void RemoveMatchTile(List<Tile> listMatchTile)
    {
        for (int i = 0; i <= 2; i++)
        {
            Destroy(listMatchTile[i].gameObject);
            // sau này sẽ nâng cấp thành pooling
        }
        listMatchTile = new List<Tile>();
    }
    private void MoveUpTilesModel(int newMatchIndex,Dictionary<Tile, Vector3> dicTileToTarget, List<Tile> listMatchTile)
    {
        for (int i = newMatchIndex; i >= newMatchIndex - 2; i--)
        {
            listMatchTile.Add(listCurrentTile[i]);
        }
        for (int i = newMatchIndex + 1; i < listCurrentTile.Count; i++)
        {
            listCurrentTile[i - 3] = listCurrentTile[i];
            dicTileToTarget.Add(listCurrentTile[i - 3], myTransform.TransformPoint(firstTilePos + tilesDistance * (i - 3)));
        }
        int lastIndex = listCurrentTile.Count-1;
        for (int i = lastIndex; i >= lastIndex-2; i--)
        {
            listCurrentTile.RemoveAt(i);
        }
    }
    private void MoveUpTilesView(Dictionary<Tile, Vector3> dicTileToTarget)
    {
        foreach(KeyValuePair<Tile,Vector3> tileToTarget  in dicTileToTarget)
        {
            tileToTarget.Key.transform.DOMove(tileToTarget.Value, 0.5f);
        }
    }

}
