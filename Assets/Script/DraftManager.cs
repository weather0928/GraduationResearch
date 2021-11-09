using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab; //カードのprefab取得

    [SerializeField] GameObject draftCanvas;
    [SerializeField] Transform cardSelectFieldR; //カードを表示する場所を取得（右側）
    [SerializeField] Transform cardSelectFieldL; //カードを表示する場所を取得（左側）

    [SerializeField] GameObject deckCanvas;
    [SerializeField] Transform cardDisplayCanvas;

    [SerializeField] int pickCount; //ピック回数
    int deckCount; //デッキの枚数（ピック回数×２）

    //選択部分に出てきているカードの情報を保持（リセット用）
    List<CardController> fieldCardList = new List<CardController>();

    //選択部分に出てきているカードのIDを保持
    List<int> fieldCardIDList = new List<int>();

    //選択したカードのIDを保持
    List<int> deckList = new List<int>();

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
        selectEnd = false;
        CreateDraftCard();
    }

    //カード生成処理
    public void CreateDraftCard()
    {
        //カードリスト初期化
        List<int> cardIDList = new List<int>();

        for (int i = 1; i <= end; i++)
        {
            cardIDList.Add(i);
        }

        int cardCount = 4;

        //カード選出処理
        while (cardCount-- > 0)
        {
            int index = Random.Range(0, cardIDList.Count);
            int cardID = cardIDList[index];
            fieldCardIDList.Add(cardID);
            cardIDList.RemoveAt(index);
        }

        //カードを左右に表示
        for (int i = 0; i < 2; i++)
        {
            //左側
            CardController leftCard = Instantiate(cardPrefab, cardSelectFieldL);
            leftCard.Init(fieldCardIDList[i]);
            fieldCardList.Add(leftCard);

            //右側
            CardController rightCard = Instantiate(cardPrefab, cardSelectFieldR);
            rightCard.Init(fieldCardIDList[i + 2]);
            fieldCardList.Add(rightCard);
        }
    }

    //選択カード保存処理
    public void cardSelect(int start,int end)
    {
        if(selectEnd == false)
        { 
            for (int i = start; i < end; i++)
            {
                deckList.Add(fieldCardIDList[i]);
            }

            fieldCardIDList.Clear();

            //デッキ完成処理
            if (deckList.Count == deckCount)
            {
                draftCanvas.SetActive(false);
                deckCanvas.SetActive(true);
                selectEnd = true;
                for (int i = 0; i < deckList.Count; i++)
                {
                    CreateCard(deckList[i], cardDisplayCanvas);
                }
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
    void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(cardID);
    }
}
