using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite ItemImage;
    public string ItemDescription;
    public GameObject Item3DModel;
    public string Category;
    public string ItemHeight;
    public string ItemWidth;
    public string ItemDepth;
}
