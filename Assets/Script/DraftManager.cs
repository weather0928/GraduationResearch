using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab; //カードのprefab取得
    [SerializeField] Transform cardSelectField; //カードを表示する場所を取得
    [SerializeField] int deckCount; //デッキの枚数

    //選択部分に出てきているカードの情報を保持（リセット用）
    List<CardController> fieldCardList = new List<CardController>();

    //選択したカードの情報を保持
    List<CardController> deckList = new List<CardController>();

    int end = 5; //カードの種類を取得（そのうち自動化したい）
    
    void Start()
    {
        if(deckCount < 1)
        {
            deckCount = 1;
        }
        CreateDraftCard();
    }

    //カード生成処理
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
            CardController card = Instantiate(cardPrefab, cardSelectField);
            card.Init(cardID);
            fieldCardList.Add(card);
            cardIDList.RemoveAt(index);
        }
    }

    //選択カード保存処理
    public void cardSelect(CardController selectCard)
    {
        deckList.Add(selectCard);
        Debug.Log(deckList[deckList.Count - 1].model.name);//テスト用
    }

    //選択画面リセット処理
    public void ResetField()
    {
        for(int i = 0;i < fieldCardList.Count;i++)
        {
            Destroy(fieldCardList[i].gameObject);
        }
        fieldCardList.Clear();
    }

    //カード生成機能（今は上と統合。分離の必要があったらこっち使う）
    /*void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, CardSelectField);
        card.Init(cardID);
    }*/
}
