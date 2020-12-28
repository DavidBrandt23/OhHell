using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpriteList", order = 1)]
public class SpriteList : ScriptableObject
{
    public List<Sprite> Sprites;

    public Sprite GetSprite(int index)
    {
        return Sprites[index];
    }
}