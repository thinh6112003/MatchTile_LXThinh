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
    [SerializeField] private bool isNotAutoInsert= false;
    [SerializeField] private bool isHighSpeed= false;
    [SerializeField] private int idTileInsert= 0;
    [SerializeField] private int matchTileNumber = 3;
    [SerializeField] private int maxCollectTile= 8 ;
    [SerializeField] private float speedSpawn= 1;
    [SerializeField] private float hideMatchTileTime = 0.15f;
    [SerializeField] private float moveToTargetTime = 0.6f;
    [SerializeField] private Vector3 firstTilePos= new Vector3(0.65f,0.2f,0f);
    [SerializeField] private Vector3 tilesDistance = new Vector3(1.455f,0f,0f);
    [SerializeField] private Vector3 tilesScaleZoom = new Vector3(1.3f,1.3f,1f);
    [SerializeField] private Vector3 tilesScaleInBox = new Vector3(1.3f,1.3f,1f);
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private List<Tile> listCurrentTile = new List<Tile>();
    private int countTileAfterMatch=0;
    private int dem = 0;
    private int demhighspeed = 0 ;
    private bool endgame = false;
    private bool fullBox = false;
    private Transform myTransform;
    private void Start()
    {
        myTransform = this.transform;
        fullBox = false;
    }
    public void Test(Tile newTile)
    {
        Debug.Log("insign test");
        StartCoroutine(AddTile(newTile));
    }
    private IEnumerator AddTile(Tile newTile)
    {
        newTile.transform.SetParent(myTransform);
        Debug.Log("insign addtile");
        newTile.isMoveToBox = true;
        MoveToInsertTile(newTile);
        if (CheckEndGame_FullBox())
        {
            Observer.Noti(Notifi.END_GAME);
        }
        while(newTile.isMoveToBox)
        {
            yield return null;
        }
        if (CheckThreeMatch(newTile))
        {
            List<Tile> listMatchTile = new List<Tile>();
            MoveLeftTiles(newTile, listMatchTile);
            HideMatchTile(listMatchTile);
            yield return new WaitForSeconds(0.2f);
            RemoveMatchTile(listMatchTile);
        }
    }
    private bool CheckEndGame_FullBox()
    {
        int tileCount = listCurrentTile.Count;
        if (tileCount == maxCollectTile) fullBox = true;
        for(int i = 2; i < listCurrentTile.Count; i++)
        {
            int spriteID_Tile_i= listCurrentTile[i].getSpriteID();
            if (spriteID_Tile_i == listCurrentTile[i-1].getSpriteID()&& spriteID_Tile_i == listCurrentTile[i - 2].getSpriteID()&&
                (i+1 == listCurrentTile.Count || spriteID_Tile_i != listCurrentTile[i +1].getSpriteID()))
            {
                tileCount -= tileCount;
            }
        }
        if (tileCount < maxCollectTile)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void MoveToInsertTile(Tile newTile)
    {
        newTile.setIndexInBox(listCurrentTile.Count);
        for (int i = listCurrentTile.Count - 1; i >= 0; i--)
        {
            if (listCurrentTile[i].getSpriteID() == newTile.getSpriteID())
            {
                newTile.setIndexInBox(i + 1);
                break;
            }
        }
        listCurrentTile.Add(null);
        if (newTile.getIndexInBox() != listCurrentTile.Count - 1)
        {
            for (int i = listCurrentTile.Count - 2; i >= newTile.getIndexInBox(); i--)
            {
                listCurrentTile[i].transform.DOMove(myTransform.TransformPoint(firstTilePos + tilesDistance * (i + 1)), moveToTargetTime)
                    .OnComplete(() => listCurrentTile[i].isMoveToBox = false);
                listCurrentTile[i + 1] = listCurrentTile[i];
                listCurrentTile[i + 1].setIndexInBox(i + 1);
            }
        }
        newTile.transform.DOMove(myTransform.TransformPoint(firstTilePos + newTile.getIndexInBox() * tilesDistance), moveToTargetTime)
            .OnComplete( () => newTile.isMoveToBox = false);

        newTile.gameObject.transform.DOScale(tilesScaleZoom, moveToTargetTime / 2f)
            .OnComplete(() =>
            {
                newTile.gameObject.transform.DOScale(tilesScaleInBox, moveToTargetTime / 2f);
            });
        listCurrentTile[newTile.getIndexInBox()] = newTile;
    }
    private bool CheckThreeMatch(Tile newTile)
    {
        //return false; // test dont check three match
        if (newTile.getIndexInBox() >= 2)
        {
            int newTileSpriteID = newTile.getSpriteID();
            if (listCurrentTile[newTile.getIndexInBox() - 1].getSpriteID() != newTileSpriteID ||
                listCurrentTile[newTile.getIndexInBox() - 2].getSpriteID() != newTileSpriteID)
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
    private void MoveLeftTiles(Tile newTile, List<Tile> listMatchTile)
    {
        fullBox = false;
        for (int i = newTile.getIndexInBox(); i > newTile.getIndexInBox() - matchTileNumber; i--)
        {
            listMatchTile.Add(listCurrentTile[i]);
        }
        for (int i = newTile.getIndexInBox() + 1; i < listCurrentTile.Count; i++)
        {
            listCurrentTile[i].transform
                .DOMove(myTransform.TransformPoint(firstTilePos + tilesDistance * (i - matchTileNumber)), moveToTargetTime)
                .OnComplete(() => listCurrentTile[i].isMoveToBox = false);
            listCurrentTile[i - matchTileNumber] = listCurrentTile[i];
            listCurrentTile[i - matchTileNumber].setIndexInBox(i - matchTileNumber);
        }
        int lastIndex = listCurrentTile.Count-1;
        for (int i = lastIndex; i > lastIndex-matchTileNumber; i--)
        {
            listCurrentTile.RemoveAt(i);
        }   
    }
    private void HideMatchTile(List<Tile> listMatchTile)
    {
        for (int i = 0; i < matchTileNumber; i++)
        {
            listMatchTile[i].gameObject.transform.DOScale(new Vector3(0, 0), hideMatchTileTime);
        }
    }
    private void RemoveMatchTile(List<Tile> listMatchTile)
    {
        for (int i = 0; i < matchTileNumber; i++)
        {
            Destroy(listMatchTile[i].gameObject);
            // sau này sẽ nâng cấp thành pooling
        }
    }
}
