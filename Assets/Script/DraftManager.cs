using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab; //カードのprefab取得
    [SerializeField] Transform cardSelectFieldR; //カードを表示する場所を取得（右側）
    [SerializeField] Transform cardSelectFieldL; //カードを表示する場所を取得（左側）
    [SerializeField] Text text; //画面上部の表示部分
    [SerializeField] int pickCount; //ピック回数
    int deckCount; //デッキの枚数（ピック回数×２）

    //選択部分に出てきているカードの情報を保持（リセット用）
    List<CardController> fieldCardList = new List<CardController>();

    //選択部分に出てきているカードのIDを保持
    List<int> fieldCardIDList = new List<int>();

    //選択したカードの情報を保持
    List<CardController> deckList = new List<CardController>();

    //選択終了フラグ
    [System.NonSerialized] public bool selectEnd;

    int end = 5; //カードの種類を取得（そのうち自動化したい）
    
    void Start()
    {
        if(pickCount < 1)
        {
            pickCount = 1;
        }
        deckCount = pickCount * 2;
        text.text = "どちらかのカードを選択してください";
        selectEnd = false;
        CreateDraftCard();
    }

    //カード生成処理
    public void CreateDraftCard()
    {
        //選択肢の数
        int choices = 2;

        //選ばれたカードのIDを保存
        while(choices-- > 0)
        {
            List<int> cardIDList = new List<int>();

            for (int i = 1; i <= end; i++)
            {
                cardIDList.Add(i);
            }

            int cardCount = 2;

            while (cardCount-- > 0)
            {
                int index = Random.Range(0, cardIDList.Count);
                int cardID = cardIDList[index];
                fieldCardIDList.Add(cardID);
                cardIDList.RemoveAt(index);
            }
        }

        //カードを左右に表示（2枚ずつ）
        for (int i = 0; i < 2; i++)
        {
            CardController card = Instantiate(cardPrefab, cardSelectFieldL);
            card.Init(fieldCardIDList[i]);
            fieldCardList.Add(card);
        }

        for (int i = 2; i < 4; i++)
        {
            CardController card = Instantiate(cardPrefab, cardSelectFieldR);
            card.Init(fieldCardIDList[i]);
            fieldCardList.Add(card);
        }
    }

    //選択カード保存処理
    public void cardSelect(CardController selectCard)
    {
        if(selectEnd == false)
        {
            deckList.Add(selectCard);

            //デッキ完成処理
            if (deckList.Count == deckCount)
            {
                selectEnd = true;
                text.text = "デッキ内容";
                /*for (int i = 0; i < deckList.Count; i++)
                {
                    CreateCard(deckList[i].model.cardID, cardSelectField);
                }*/
            }
        }
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

    //カード生成機能（デッキ表示用）
    /*void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, cardSelectField);
        card.Init(cardID);
    }*/
}
