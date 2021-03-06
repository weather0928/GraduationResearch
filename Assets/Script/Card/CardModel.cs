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
    public float evaluation;
    public bool canAttack = false;
    //public Sprite icon; //画像表示（後日追加）

    public CardModel(int selectCardID)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardList/Card" + selectCardID);

        cardID = cardEntity.cardID;
        name = cardEntity.name;
        cost = cardEntity.cost;
        power = cardEntity.power;
        hp = cardEntity.hp;
        evaluation = cardEntity.evaluation;
        //icon = cardEntity.icon;
    }
}
