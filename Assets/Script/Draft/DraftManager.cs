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
    [System.NonSerialized] public int[] playerDeckCost = new int[8]; //マナカーブを確認するためのもの
    int[] NPC1deckCost = new int[8]; //マナカーブを確認するためのもの
    int[] NPC2deckCost = new int[8]; //マナカーブを確認するためのもの

    //NPCデッキ制作用。どれくらい評価を変動させるのかを入れる
    [SerializeField] int evaluationFluctuation;

    //選択終了フラグ
    [System.NonSerialized] public bool selectEnd;

    [SerializeField] bool NPCDeckBuildFlag;
    int NPCChangeEvaluation;

    int Cost2Count;

    int end = 63; //カードの種類を取得（そのうち自動化したい）
    
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
    public void cardSelect(List<CardController> cardList,Deck deck,int[] manaCost)
    {
        if(selectEnd == false)
        { 
            for (int i = 0; i < cardList.Count; i++)
            {
                deck.cardList.Add(cardList[i].model.cardID);

                //デッキ内マナコスト反映処理
                if(cardList[i].model.cost < 8)
                {
                    manaCost[cardList[i].model.cost - 1]++;
                }
                else
                {
                    manaCost[7]++;
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
            float leftSelectEvaluation = 0;
            float rightSelectEvaluation = 0;

            for(int i = 0;i < 2;i++)
            {
                leftSelectEvaluation += leftCardList[i].model.evaluation;
                rightSelectEvaluation += rightCardList[i].model.evaluation;
            }

            NPC1Pick(leftSelectEvaluation, rightSelectEvaluation, NPCDeck1);
            //NPC2Pick(leftSelectEvaluation, rightSelectEvaluation, NPCDeck2);

            if(NPCDeck1.cardList.Count == deckCount)
            {
                draftCanvas.SetActive(false);
                deckCanvas.SetActive(true);

                //デッキを再起動後も保存する処理
                EditorUtility.SetDirty(NPCDeck1);
                //EditorUtility.SetDirty(NPCDeck2);
                AssetDatabase.SaveAssets();

                selectEnd = true;
            }

            ResetField();

            if (selectEnd == false)
            {
                //カード再生成処理
                CreateDraftCard();
            }
            else
            {
                Debug.Log("1:" + NPC1deckCost[0] + " 2:" + NPC1deckCost[1] + " 3:" + NPC1deckCost[2] + " 4:" + NPC1deckCost[3]
                    + " 5:" + NPC1deckCost[4] + " 6:" + NPC1deckCost[5] + " 7:" + NPC1deckCost[6] + " 8over:" + NPC1deckCost[7]);
                ManaCarveEvaluation();
            }
        }
    }

    //NPCの思考部分
    //マナカーブの調整をするNPC
    void NPC1Pick(float leftEvaluation, float rightEvaluation, Deck deck)
    {
        if (NPCDeck1.cardList.Count >= NPCChangeEvaluation)
        {
            leftEvaluation = EvaluationFluctuation(leftCardList,leftEvaluation, NPC1deckCost);
            rightEvaluation = EvaluationFluctuation(rightCardList, rightEvaluation, NPC1deckCost);
        }

        if (leftEvaluation >= rightEvaluation)
        {
            cardSelect(leftCardList,deck,NPC1deckCost);
        }
        else if (leftEvaluation < rightEvaluation)
        {
            cardSelect(rightCardList,deck,NPC1deckCost);
        }
    }

    //ランダムなカードをピックするNPC
    void NPC2Pick(float leftEvaluation, float rightEvaluation, Deck deck)
    {
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            cardSelect(leftCardList, deck,NPC2deckCost);
        }
        else if (random == 1)
        {
            cardSelect(rightCardList, deck,NPC2deckCost);
        }
    }

    //マナカーブ実験用
    float EvaluationFluctuation(List<CardController> cardList, float evaluationNum, int[] deckManaCost)
    {
        float fluctuationNum = 0;

        for (int i = 0; i < 2; i++)
        {
            switch (cardList[i].model.cost)
            {
                case 1:
                    if (deckManaCost[0] >= 3)
                    {
                        fluctuationNum = evaluationNum - evaluationFluctuation;
                    }
                    break;
                case 2:
                    if (deckManaCost[1] < 6)
                    {
                        fluctuationNum = evaluationNum + evaluationFluctuation;
                    }
                    else if (deckManaCost[1] >= 9)
                    {
                        fluctuationNum = evaluationNum - evaluationFluctuation;
                    }
                    break;
                case 3:
                    if (deckManaCost[2] < 4)
                    {
                        fluctuationNum = evaluationNum + evaluationFluctuation;
                    }
                    else if (deckManaCost[2] >= 6)
                    {
                        fluctuationNum = evaluationNum - evaluationFluctuation;
                    }
                    break;
                case 4:
                    if (deckManaCost[3] < 3)
                    {
                        fluctuationNum = evaluationNum + evaluationFluctuation;
                    }
                    else if (deckManaCost[3] >= 6)
                    {
                        fluctuationNum = evaluationNum - evaluationFluctuation;
                    }
                    break;
                case 5:
                    if (deckManaCost[4] < 2)
                    {
                        fluctuationNum = evaluationNum + evaluationFluctuation;
                    }
                    else if (deckManaCost[4] >= 4)
                    {
                        fluctuationNum = evaluationNum - evaluationFluctuation;
                    }
                    break;
                case 6:
                    if (deckManaCost[5] < 1)
                    {
                        fluctuationNum = evaluationNum + evaluationFluctuation;
                    }
                    else if (deckManaCost[5] >= 4)
                    {
                        fluctuationNum = evaluationNum - evaluationFluctuation;
                    }
                    break;
                case 7:
                    if (deckManaCost[6] < 1)
                    {
                        fluctuationNum = evaluationNum + evaluationFluctuation;
                    }
                    else if (deckManaCost[6] >= 4)
                    {
                        fluctuationNum = evaluationNum - evaluationFluctuation;
                    }
                    break;
                default:
                    if (deckManaCost[7] < 1)
                    {
                        fluctuationNum = evaluationNum + evaluationFluctuation;
                    }
                    else if (deckManaCost[7] >= 4)
                    {
                        fluctuationNum = evaluationNum - evaluationFluctuation;
                    }
                    break;
            }
        }

        return fluctuationNum;
    }

    //デッキ評価
    void ManaCarveEvaluation()
    {
        int deckEvalution = 0;

        if(NPC1deckCost[0] >=0 && NPC1deckCost[0] <=3)
        {
            deckEvalution++;
        }
        else
        {
            //deckEvalution--;
        }

        if (NPC1deckCost[1] >= 6 && NPC1deckCost[1] <= 9)
        {
            deckEvalution++;
        }
        else
        {
            //deckEvalution--;
        }

        if (NPC1deckCost[2] >= 4 && NPC1deckCost[2] <= 6)
        {
            deckEvalution++;
        }
        else
        {
            //deckEvalution--;
        }

        if (NPC1deckCost[3] >= 3 && NPC1deckCost[3] <= 6)
        {
            deckEvalution++;
        }
        else
        {
            //deckEvalution--;
        }

        if (NPC1deckCost[4] >= 2 && NPC1deckCost[4] <= 4)
        {
            deckEvalution++;
        }
        else
        {
            //deckEvalution--;
        }

        for (int i = 5; i < 8;i++)
        {
            if (NPC1deckCost[i] >= 1 && NPC1deckCost[i] <= 4)
            {
                deckEvalution++;
            }
            else
            {
                //deckEvalution--;
            }
        }

        Debug.Log(deckEvalution);
    }
}
