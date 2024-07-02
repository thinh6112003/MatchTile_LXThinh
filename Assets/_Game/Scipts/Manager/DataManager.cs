using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public SpriteSO spriteSO;
    void Awake()
    {
        Instance = this;
    }
}