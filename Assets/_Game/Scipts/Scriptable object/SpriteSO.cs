using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteSO", menuName = "Scriptable Object/SpriteSO", order = 1)]
public class SpriteSO : ScriptableObject
{
    public List<Sprite> sprites;
}
