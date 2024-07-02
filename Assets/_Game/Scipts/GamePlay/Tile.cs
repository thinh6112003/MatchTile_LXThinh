using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private int maxTile = 24;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isExposed= false;
    private int spriteID = 0;
    public int getSpriteID()
    {
        return spriteID;
    }
    public void setTile(int id = -1)
    {
        if(id==-1)
            spriteID = Random.Range(0, maxTile);
        else 
            spriteID = id;
        spriteRenderer.sprite = DataManager.Instance.spriteSO.sprites[spriteID];
    }
}

