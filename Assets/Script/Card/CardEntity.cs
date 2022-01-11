using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEntity",menuName = "Create CardEntity")]

public class CardEntity : ScriptableObject
{
    public int cardID;
    public new string name;
    public int cost;
    public int power;
    public int hp;
    public float evaluation;
    //public Sprite icon; //‰æ‘œ•\¦iŒã“ú’Ç‰Á?j
}
