using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Card Class
[CreateAssetMenu]
public class Card : ScriptableObject
{
    public int Value;
    public Sprite CardFace;
    public string Suit;
    public bool isAce;
}
