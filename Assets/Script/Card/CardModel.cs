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
    //public Sprite icon; //âÊëúï\é¶Åiå„ì˙í«â¡Åj

    public CardModel(int selectCardID)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardList/Test" + selectCardID);

        cardID = cardEntity.cardID;
        name = cardEntity.name;
        cost = cardEntity.cost;
        power = cardEntity.power;
        hp = cardEntity.hp;
        //icon = cardEntity.icon;
    }
}
