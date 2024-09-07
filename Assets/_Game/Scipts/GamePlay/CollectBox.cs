using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CollectBox : MonoBehaviour
{
    [SerializeField] private int matchTileNumber = 3;
    [SerializeField] private int maxCollectTile = 8;
    [SerializeField] private float hideMatchTileTime = 0.15f;
    [SerializeField] private float moveToTargetTime = 0.4f;
    [SerializeField] private SpriteRenderer hideEffect;
    [SerializeField] private Vector3 firstTilePos = new Vector3(0.65f, 0.2f, 0f);
    [SerializeField] private Vector3 tilesDistance = new Vector3(1.455f, 0f, 0f);
    [SerializeField] private Vector3 tilesScaleZoom = new Vector3(1.3f, 1.3f, 1f);
    [SerializeField] private Vector3 tilesScaleInBox = new Vector3(1.3f, 1.3f, 1f);
    [SerializeField] private Vector3 tileEffectOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private GameObject tileEffect;
    [SerializeField] private List<Tile> listCurrentTile = new List<Tile>();
    private Transform myTransform;
    private void Awake()
    {
        myTransform = this.transform;
    }
    internal void Init()
    {
        for (int i = 0; i < listCurrentTile.Count; i++)
        {
            Destroy(listCurrentTile[i].gameObject);
        }
        listCurrentTile = new List<Tile>();
        hideEffect.DOFade(0, 0);
    }
    internal void AddTile(Tile newTile)
    {
        newTile.InitMoveToBox(myTransform);
        StartCoroutine(AddTileCoroutine(newTile));
    }
    private IEnumerator AddTileCoroutine(Tile newTile)
    {
        MoveToInsertTile(newTile);
        if (CheckEndGame_FullBox()) Observer.Noti(Notifi.DEFEAT_GAME);
        while (newTile.isMoveToBox) yield return null;
        if (CheckThreeMatch(newTile))
        {
            List<Tile> listMatchTile = new List<Tile>();
            MoveLeftTiles(newTile, listMatchTile);
            HideMatchTile(listMatchTile);
        }
    }
    private bool CheckEndGame_FullBox()   // check lose game
    {
        int tileCount = listCurrentTile.Count;

        for (int i = 2; i < listCurrentTile.Count; i++)
        {
            int spriteID_Tile_i = listCurrentTile[i].getSpriteID();
            if (spriteID_Tile_i == listCurrentTile[i - 1].getSpriteID() && spriteID_Tile_i == listCurrentTile[i - 2].getSpriteID() &&
                (i + 1 == listCurrentTile.Count || spriteID_Tile_i != listCurrentTile[i + 1].getSpriteID()))
            {
                tileCount -= tileCount;
            }
        }

        if (tileCount < maxCollectTile) return false;

        return true;
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
                    .OnComplete(() => listCurrentTile[i].isMoveToBox = false).SetAutoKill(true);
                listCurrentTile[i + 1] = listCurrentTile[i];
                listCurrentTile[i + 1].setIndexInBox(i + 1);
            }
        }
        newTile.transform.DOMove(myTransform.TransformPoint(firstTilePos + newTile.getIndexInBox() * tilesDistance), moveToTargetTime)
            .OnComplete(() => newTile.isMoveToBox = false)
            .SetAutoKill(true);

        newTile.gameObject.transform.DOScale(tilesScaleZoom, moveToTargetTime / 2f)
            .SetEase(Ease.OutQuint)
            .OnComplete(() =>
            {
                newTile.gameObject.transform.DOScale(tilesScaleInBox, moveToTargetTime / 2f)
                    .SetEase(Ease.InQuint).SetAutoKill(true);
            })
            .SetAutoKill(true);
        listCurrentTile[newTile.getIndexInBox()] = newTile;
    }
    private bool CheckThreeMatch(Tile newTile)
    {
        //return false; // test don't check three match
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
        int lastIndex = listCurrentTile.Count - 1;
        for (int i = lastIndex; i > lastIndex - matchTileNumber; i--)
        {
            listCurrentTile.RemoveAt(i);
        }
    }
    private void HideMatchTile(List<Tile> listMatchTile)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.matchTile);
        for (int i = 0; i < matchTileNumber; i++)
        {
            listMatchTile[i].gameObject.transform.DOScale(new Vector3(0, 0), hideMatchTileTime);
            GameObject newTileEffect = Instantiate(tileEffect);
            newTileEffect.transform.position = listMatchTile[i].gameObject.transform.position + tileEffectOffset;
            Destroy(newTileEffect, 1.5f);
            Destroy(listMatchTile[i].gameObject, 1.5f);
        }
    }
    internal void HideBox()
    {
        hideEffect.gameObject.SetActive(true);
        hideEffect.DOFade(0.6f, 0.3f);
    }
}
