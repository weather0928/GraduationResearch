using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardModel
{
    public int cardID;
    public string name;
    public int cost;
    public int power;
    public int hp;
    //public Sprite icon; //‰æ‘œ•\¦iŒã“ú’Ç‰Áj

    public CardModel(int cardID)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardList/Test" + cardID);

        cardID = cardEntity.cardID;
        name = cardEntity.name;
        cost = cardEntity.cost;
        power = cardEntity.power;
        hp = cardEntity.hp;
        //icon = cardEntity.icon;
    }
}
