using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCGameManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab;
    [SerializeField] CardController handCardPrefab;
    [SerializeField] Transform NPC1Field;
    [SerializeField] GameObject NPC1ManaCost;
    [SerializeField] Transform NPC2Field;
    [SerializeField] GameObject NPC2ManaCost;

    [SerializeField] Transform NPC1HandCard;
    [SerializeField] Transform NPC2HandCard;

    bool isNPC1Turn = true;
    bool startFlag = true;
    Deck NPCDeck1, NPCDeck2;
    List<CardController> NPC1HandList,NPC2HandList;

    int playerCost, playerMaxCost, enemyCost, enemyMaxCost;

    void Start()
    {
        NPCDeck1 = Resources.Load<Deck>("Deck/NPC1");
        NPCDeck2 = Resources.Load<Deck>("Deck/NPC2");
        for (int i = 0;i < 3;i++)
        {
            DrowCard(NPCDeck1, NPC1HandCard);
            DrowCard(NPCDeck2, NPC2HandCard);
        }
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

    void TurnCalc()
    {
        Image NPC1Mana = NPC1ManaCost.GetComponent<Image>();
        Image NPC2Mana = NPC2ManaCost.GetComponent<Image>();

        if (isNPC1Turn)
        {
            NPC1Mana.color = Color.yellow;
            NPC2Mana.color = Color.white;

            NPCTurn(NPCDeck1,NPC1HandCard,NPC1Field);
        }
        else
        {
            NPC1Mana.color = Color.white;
            NPC2Mana.color = Color.yellow;

            NPCTurn(NPCDeck2,NPC2HandCard,NPC2Field);
        }
    }

    public void ChangeTurn()
    {
        isNPC1Turn = !isNPC1Turn;
        TurnCalc();
    }

    void NPCTurn(Deck deck,Transform hand,Transform field)
    {
        DrowCard(deck, hand);

        CardController[] fieldCardList = field.GetComponentsInChildren<CardController>();

        if(fieldCardList.Length < 5)
        {
            CardController[] handCardList = hand.GetComponentsInChildren<CardController>();
            SummonCard(handCardList[0], field);
            Destroy(handCardList[0]);
        }
    }
}
