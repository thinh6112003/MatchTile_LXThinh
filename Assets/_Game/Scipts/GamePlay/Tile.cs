using UnityEngine;
using static UnityEngine.UI.Image;
public class Tile : MonoBehaviour
{
    [SerializeField] private int maxTile = 24;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isExposed= false; // hi?n là exposed
    [SerializeField] private int indexInBox = -1;
    public bool isMoveToBox = true;
    public string control = "";
    private int spriteID = 0;
    private SpriteRenderer mySpriteRenderer;
    private Transform myTransform;
    private void Awake()
    {
        mySpriteRenderer = this.GetComponent<SpriteRenderer>();
        myTransform = this.GetComponent<Transform>();
    }
    private void Start()
    {
        setTile();
    }
    public int getSpriteID()
    {
        return spriteID;
    }
    public int getIndexInBox()
    {
        return indexInBox;
    }
    public void setIndexInBox(int index)
    {
        indexInBox = index;
    }
    public void setTile(int id = -1)
    {
        if(id==-1)
            spriteID = Random.Range(0, maxTile);
        else 
            spriteID = id;
        spriteRenderer.sprite = DataManager.Instance.spriteSO.sprites[spriteID];
        spriteRenderer.sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder+1;
        spriteRenderer.color = this.GetComponent<SpriteRenderer>().color;
    }
    public void setTile(Vector3 localPos,Color color,ref int sortingOder, LayerMask layerMark)
    {
        this.transform.localPosition = localPos;
        mySpriteRenderer.sortingOrder = sortingOder++;
        mySpriteRenderer.color = color;
        this.gameObject.layer = layerMark;
    }
    public void InitMoveToBox(Transform boxTransform)
    {
        this.transform.SetParent(boxTransform);
        this.isMoveToBox = true;
        mySpriteRenderer.sortingOrder = 1000;
        spriteRenderer.sortingOrder = 1001;
    }
}

