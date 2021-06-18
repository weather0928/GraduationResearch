using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab;
    [SerializeField] Transform CardSelectField;

    List<CardController> cardList = new List<CardController>();

    int end = 5;

    void Start()
    {
        CreateDraftCard();
    }

    public void CreateDraftCard()
    {
        List<int> cardIDList = new List<int>();

        for (int i = 1; i <= end; i++)
        {
            cardIDList.Add(i);
        }

        int count = 3;

        while (count-- > 0)
        {
            int index = Random.Range(0, cardIDList.Count);
            int cardID = cardIDList[index];
            CardController card = Instantiate(cardPrefab, CardSelectField);
            card.Init(cardID);
            cardList.Add(card);
            cardIDList.RemoveAt(index);
        }
    }

    public void ResetField()
    {
        for(int i = 0;i < cardList.Count;i++)
        {
            Destroy(cardList[i].gameObject);
        }
        cardList.Clear();
    }

    //カード生成機能（今は上と統合。分離の必要があったらこっち使う）
    /*void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, CardSelectField);
        card.Init(cardID);
    }*/
}
