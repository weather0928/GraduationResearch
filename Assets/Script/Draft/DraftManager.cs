using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

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

    //左側に出てきているカードの情報を保持
    [System.NonSerialized] public List<CardController> leftCardList = new List<CardController>();

    //右側に出てきているカードの情報を保持
    [System.NonSerialized] public List<CardController> rightCardList = new List<CardController>();

    //デッキ情報
    Deck playerDeck;
    Deck NPCDeck1, NPCDeck2;

    //選択終了フラグ
    [System.NonSerialized] public bool selectEnd;

    [SerializeField] bool NPCDeckBuildFlag;
    int NPCChangeEvaluation;

    int Cost2Count;

    int end = 12; //カードの種類を取得（そのうち自動化したい）
    
    void Start()
    {
        if(pickCount < 1)
        {
            pickCount = 1;
        }
        deckCount = pickCount * 2;
        selectEnd = false;
        CreateDraftCard();
        if(NPCDeckBuildFlag == true)
        {
            NPCDeck1 = Resources.Load<Deck>("Deck/NPC1");
            NPCDeck2 = Resources.Load<Deck>("Deck/NPC2");
            NPCDeck1.cardList.Clear();
            NPCDeck2.cardList.Clear();
            NPCChangeEvaluation = deckCount / 3 * 2;
            NPCDeckBuild();
        }
        else
        {
            playerDeck = Resources.Load<Deck>("Deck/Test");
            playerDeck.cardList.Clear();
        }
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

        //カードを左右に表示
        for (int i = 0; i < 2; i++)
        {
            int index, cardID;

            //左側
            index = Random.Range(0, cardIDList.Count);
            cardID = cardIDList[index];
            CardController leftCard = CreatePickCard(cardID, cardSelectFieldL);
            leftCardList.Add(leftCard);
            cardIDList.RemoveAt(index);

            //右側
            index = Random.Range(0, cardIDList.Count);
            cardID = cardIDList[index];
            CardController rightCard = CreatePickCard(cardID, cardSelectFieldR);
            rightCardList.Add(rightCard);
            cardIDList.RemoveAt(index);
        }
    }

    //選択カード保存処理
    public void cardSelect(List<CardController> cardList,Deck deck)
    {
        if(selectEnd == false)
        { 
            for (int i = 0; i < cardList.Count; i++)
            {
                deck.cardList.Add(cardList[i].model.cardID);
                if(cardList[i].model.cost == 2)
                {
                    Cost2Count++;
                }
            }

            //デッキ完成処理
            //プレイヤーがデッキを作る場合
            if(NPCDeckBuildFlag == false)
            {
                if (deck.cardList.Count == deckCount)
                {
                    draftCanvas.SetActive(false);
                    deckCanvas.SetActive(true);
                    selectEnd = true;

                    //デッキを再起動後も保存する処理
                    EditorUtility.SetDirty(playerDeck);
                    AssetDatabase.SaveAssets();
                    /*for (int i = 0; i < deck.cardList.Count; i++)
                    {
                        CreateCard(deck.cardList[i], cardDisplayCanvas);
                    }*/
                    Debug.Log(Cost2Count);
                }
            }
        }
    }

    //選択画面リセット処理
    public void ResetField()
    {
        for(int i = 0;i < 2;i++)
        {
            Destroy(leftCardList[i].gameObject);
            Destroy(rightCardList[i].gameObject);
        }
        leftCardList.Clear();
        rightCardList.Clear();
    }

    //カード生成機能（ピック画面用）
    CardController CreatePickCard(int cardID,Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(cardID);
        return card;
    }

    //カード生成機能（デッキ表示用）
    void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(cardID);
    }

    //NPCデッキ制作処理(共通部分)
    void NPCDeckBuild()
    {
        while(selectEnd == false)
        {
            int leftSelectEvaluation = 0;
            int rightSelectEvaluation = 0;

            for(int i = 0;i < 2;i++)
            {
                leftSelectEvaluation += leftCardList[i].model.evaluation;
                rightSelectEvaluation += rightCardList[i].model.evaluation;
            }

            NPC1Pick(leftSelectEvaluation, rightSelectEvaluation, NPCDeck1);
            NPC2Pick(leftSelectEvaluation, rightSelectEvaluation, NPCDeck2);

            if(NPCDeck1.cardList.Count == deckCount)
            {
                draftCanvas.SetActive(false);
                deckCanvas.SetActive(true);

                //デッキを再起動後も保存する処理
                EditorUtility.SetDirty(NPCDeck1);
                EditorUtility.SetDirty(NPCDeck2);
                AssetDatabase.SaveAssets();

                selectEnd = true;
                /*for (int i = 0; i < deck.cardList.Count; i++)
                {
                    CreateCard(deck.cardList[i], cardDisplayCanvas);
                }*/
                Debug.Log(Cost2Count);
            }

            ResetField();

            if (selectEnd == false)
            {
                //カード再生成処理
                CreateDraftCard();
            }
        }
    }

    //NPCの思考部分
    //2コスの枚数調整をするNPC
    void NPC1Pick(int leftEvaluation, int rightEvaluation, Deck deck)
    {
        if (NPCDeck1.cardList.Count >= NPCChangeEvaluation
                && Cost2Count < deckCount / 3)
        {
            for (int i = 0; i < 2; i++)
            {
                if (leftCardList[i].model.cost == 2)
                {
                    leftEvaluation += 5;
                }
                if (rightCardList[i].model.cost == 2)
                {
                    rightEvaluation += 5;
                }
            }
        }

        if (leftEvaluation >= rightEvaluation)
        {
            cardSelect(leftCardList,deck);
        }
        else if (leftEvaluation < rightEvaluation)
        {
            cardSelect(rightCardList,deck);
        }
    }

    //ランダムなカードをピックするNPC
    void NPC2Pick(int leftEvaluation, int rightEvaluation, Deck deck)
    {
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            cardSelect(leftCardList, deck);
        }
        else if (random == 1)
        {
            cardSelect(rightCardList, deck);
        }
    }
}
