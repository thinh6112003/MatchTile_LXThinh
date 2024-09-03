using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMap : MonoBehaviour
{
    [SerializeField] List<GameObject> buttonList = new List<GameObject>();
    [SerializeField] Sprite onSprite;
    private void OnEnable()
    {
        for (int i = 1; i <= buttonList.Count; i++)
        {
            if (i <= DataManager.dynamicData.currentLevel)
            {
                buttonList[i - 1].GetComponent<UnityEngine.UI.Image>().sprite = onSprite;
                buttonList[i - 1].GetComponent<Button>().enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
