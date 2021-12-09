using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab;
    [SerializeField] Transform playerHand;
    [SerializeField] Transform playerField;
    [SerializeField] Transform enemyField;

    bool isPlayerTurn = true;
    Deck NPCDeck1, NPCDeck2;
    List<CardController> playerHandList,enemyHandList;

    void Start()
    {
        NPCDeck1 = Resources.Load<Deck>("Deck/NPC1");
        NPCDeck2 = Resources.Load<Deck>("Deck/NPC2");
        for (int i = 0;i < 3;i++)
        {
            CreateCard(NPCDeck1.cardList[i], playerHand);
        }
    }

    void DrowCard(Deck deck)
    {
        if (deck.cardList.Count == 0)
        {
            return;
        }

        int cardID = deck.cardList[0];
        deck.cardList.RemoveAt(0);
    }

    void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(cardID);
    }
}
