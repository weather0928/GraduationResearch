using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NPCGameManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab;
    [SerializeField] CardController handCardPrefab;

    [SerializeField] Transform NPC1Field;
    [SerializeField] GameObject NPC1ManaCost;
    [SerializeField] Text NPC1HPText;

    [SerializeField] Transform NPC2Field;
    [SerializeField] GameObject NPC2ManaCost;
    [SerializeField] Text NPC2HPText;

    [SerializeField] Transform NPC1HandCard;
    [SerializeField] Transform NPC2HandCard;

    bool isNPC1Turn = true;
    bool startFlag = true;
    Deck NPCDeck1, NPCDeck2;
    List<CardController> NPC1HandList,NPC2HandList;

    int NPC1MAXMana, NPC1Mana, NPC2MAXMana, NPC2Mana;

    int NPC1HP, NPC2HP;

    void Start()
    {
        NPCDeck1 = Resources.Load<Deck>("Deck/NPC1");
        NPCDeck2 = Resources.Load<Deck>("Deck/NPC2");
        for (int i = 0;i < 3;i++)
        {
            DrowCard(NPCDeck1, NPC1HandCard);
            DrowCard(NPCDeck2, NPC2HandCard);
        }

        NPC1MAXMana = 0;
        NPC1Mana = 0;
        NPC2MAXMana = 0;
        NPC2Mana = 0;

        NPC1HP = 20;
        NPC2HP = 20;

        startFlag = false;

        TurnCalc();
    }

    //カードをドローする処理
    void DrowCard(Deck deck,Transform hand)
    {
        if (deck.cardList.Count == 0)
        {
            return;
        }

        int cardID = deck.cardList[0];
        deck.cardList.RemoveAt(0);

        CardController[] handCardList = hand.GetComponentsInChildren<CardController>();

        if (handCardList.Length < 9)
        {
            CardController card = Instantiate(handCardPrefab, hand);
            card.Init(cardID);
        }
    }

    //モンスターを場に出す処理
    void SummonCard(CardController sumonCard, Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(sumonCard.model.cardID);
    }

    //ターン切り替え処理
    void TurnCalc()
    {
        Image NPC1ManaImage = NPC1ManaCost.GetComponent<Image>();
        Text NPC1ManaText = NPC1ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();
        Image NPC2ManaImage = NPC2ManaCost.GetComponent<Image>();
        Text NPC2ManaText = NPC2ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();

        if (isNPC1Turn)
        {
            NPC1ManaImage.color = Color.yellow;
            NPC2ManaImage.color = Color.white;

            if (NPC1MAXMana < 10)
            {
                NPC1MAXMana++;
            }
            NPC1Mana = NPC1MAXMana;

            NPC1ManaText.text = NPC1Mana + "/" + NPC1MAXMana;

            NPC1Turn();
        }
        else
        {
            NPC1ManaImage.color = Color.white;
            NPC2ManaImage.color = Color.yellow;

            if (NPC2MAXMana < 10)
            {
                NPC2MAXMana++;
            }
            NPC2Mana = NPC2MAXMana;

            NPC2ManaText.text = NPC2Mana + "/" + NPC2MAXMana;

            NPC2Turn();
        }
    }

    //ターン切り替えフラグ処理
    public void ChangeTurn()
    {
        isNPC1Turn = !isNPC1Turn;
        TurnCalc();
    }

    //NPC1（画面手前）のターン処理
    void NPC1Turn()
    {
        DrowCard(NPCDeck1, NPC1HandCard);

        CardController[] NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(NPC1FieldCardList, true);

        CardController[] handCardList = NPC1HandCard.GetComponentsInChildren<CardController>();

        Text NPC1ManaText = NPC1ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();

        for (int i = 0; i < handCardList.Length; i++)
        {
            if (handCardList[i].model.cost <= NPC1Mana && NPC1FieldCardList.Length < 5)
            {
                SummonCard(handCardList[i], NPC1Field);
                NPC1Mana -= handCardList[i].model.cost;
                Destroy(handCardList[i].gameObject);
                NPC1ManaText.text = NPC1Mana + "/" + NPC1MAXMana;
            }
        }

        NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();

        while(Array.Exists(NPC1FieldCardList, card => card.model.canAttack))
        {
            CardController[] NPC1CanAttackCardList = Array.FindAll(NPC1FieldCardList, card => card.model.canAttack);
            CardController attackCard = NPC1CanAttackCardList[0];

            AttackToLeader(attackCard, false);

            NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();
        }
    }

    //NPC2（画面奥）のターン処理
    void NPC2Turn()
    {
        DrowCard(NPCDeck2, NPC2HandCard);

        CardController[] NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();
        CardController[] handCardList = NPC2HandCard.GetComponentsInChildren<CardController>();

        Text NPC2ManaText = NPC2ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();

        for (int i = 0; i < handCardList.Length; i++)
        {
            if (handCardList[i].model.cost <= NPC2Mana && NPC2FieldCardList.Length < 5)
            {
                SummonCard(handCardList[i], NPC2Field);
                NPC2Mana -= handCardList[i].model.cost;
                Destroy(handCardList[i].gameObject);
                NPC2ManaText.text = NPC2Mana + "/" + NPC2MAXMana;
            }
        }

        if (NPC2FieldCardList.Length >= 1)
        {
            for (int i = 0; i < NPC2FieldCardList.Length; i++)
            {
                //AttackToLeader(fieldCardList[i], false);
            }
        }
    }

    //攻撃可能処理
    void SetAttackableFieldCard(CardController[] cardList, bool canAttack)
    {
        foreach (CardController card in cardList)
        {
            card.model.canAttack = canAttack;
        }
    }

    //モンスターへの攻撃処理
    void CardBattle(CardController attackCard, CardController defenceCard)
    {
        defenceCard.model.hp -= attackCard.model.power;
        attackCard.model.hp -= defenceCard.model.power;

        if(defenceCard.model.hp <= 0)
        {
            Destroy(defenceCard.gameObject);
        }
        if(attackCard.model.hp <= 0)
        {
            Destroy(attackCard.gameObject);
        }
    }

    //リーダーへの攻撃処理
    void AttackToLeader(CardController attackCard, bool isNPC1Card)
    {
        if(attackCard.model.canAttack == false)
        {
            return;
        }

        if(isNPC1Card == true)
        {
            NPC2HP -= attackCard.model.power;
        }
        else
        {
            NPC1HP -= attackCard.model.power;
        }
        ShowLeaderHP();
    }

    //リーダーの体力表示処理
    void ShowLeaderHP()
    {
        NPC1HPText.text = NPC1HP.ToString();
        NPC2HPText.text = NPC2HP.ToString();
    }
}
