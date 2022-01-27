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

    int end = 63; //カードの種類を取得（そのうち自動化したい）

    //実験用変数
    //それぞれの選択肢の評価が近い時に計測する変数群
    bool NPC1LeftSelectFlag = false; //NPC1が左の選択肢を選んだ時にtrueになる
    bool NPC2LeftSelectFlag = false; //NPC2が左の選択肢を選んだ時にtrueになる
    int NPCDifSelect = 0; //2つのNPCが別々の選択肢を行った回数を記録
    int NPC1RandomCount = 0; //NPC1がランダムを使った回数
    int NPC2RandomCount = 0; //NPC2がランダムを使った回数
    float NPC1DeckEvaluation = 0;
    float NPC2DeckEvaluation = 0;
    float NPC1SumEvaluation = 0;
    float NPC2SumEvaluation = 0;

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
            NPC1Pick(NPCDeck1);
            NPC2Pick(NPCDeck2);

            //実験用
            if((NPC1LeftSelectFlag == true && NPC2LeftSelectFlag == false) 
                || (NPC1LeftSelectFlag == false && NPC2LeftSelectFlag == true))
            {
                NPCDifSelect++;
            }
            NPC1LeftSelectFlag = false;
            NPC2LeftSelectFlag = false;


            if (NPCDeck1.cardList.Count == deckCount)
            {
                draftCanvas.SetActive(false);
                deckCanvas.SetActive(true);

                //デッキを再起動後も保存する処理
                EditorUtility.SetDirty(NPCDeck1);
                EditorUtility.SetDirty(NPCDeck2);
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
                /*Debug.Log("1:" + NPC1deckCost[0] + " 2:" + NPC1deckCost[1] + " 3:" + NPC1deckCost[2] + " 4:" + NPC1deckCost[3]
                    + " 5:" + NPC1deckCost[4] + " 6:" + NPC1deckCost[5] + " 7:" + NPC1deckCost[6] + " 8over:" + NPC1deckCost[7]);*/
                ManaCarveEvaluation(NPC1deckCost);
                ManaCarveEvaluation(NPC2deckCost);

                Debug.Log("NPC1SumEvaluation:" + NPC1SumEvaluation);
                Debug.Log("NPC1RandomCount:" + NPC1RandomCount);
                Debug.Log("NPC2SumEvaluation:" + NPC2SumEvaluation);
                Debug.Log("NPC2RandomCount:" + NPC2RandomCount);
                Debug.Log("NPCDifSelect:" + NPCDifSelect);
            }
        }
    }

    //NPCの思考部分
    //提案手法両方組み合わせたNPC
    void NPC1Pick(Deck deck)
    {
        //左側の選択肢
        float leftMinEvaluation = 100f; //最低評価のカードを計測
        float leftMaxEvaluation = -100f; //最大評価のカードを計測
        float leftTotalEvaluation = 0f; //選択肢の評価合計値

        //右側の選択肢
        float rightMinEvaluation = 100f;
        float rightMaxEvaluation = -100f;
        float rightTotalEvaluation = 0f;

        if (NPCDeck1.cardList.Count >= NPCChangeEvaluation)　//マナカーブの調整のために評価を変える処理
        {
            for (int i = 0;i < 2;i++)
            {
                //カードの評価を変動させた値を保存する
                float leftCardEvaluation; //左側のカード
                float rightCardEvaluation; //右側のカード

                //評価を変動させる処理
                //左側
                leftCardEvaluation = EvaluationFluctuation(leftCardList[i], NPC1deckCost);
                leftTotalEvaluation += leftCardEvaluation;
                if (leftMinEvaluation > leftCardEvaluation) leftMinEvaluation = leftCardEvaluation;
                if (leftMaxEvaluation < leftCardEvaluation) leftMaxEvaluation = leftCardEvaluation;

                //右側
                rightCardEvaluation = EvaluationFluctuation(rightCardList[i], NPC1deckCost);
                rightTotalEvaluation += rightCardEvaluation;
                if (rightMinEvaluation > rightCardEvaluation) rightMinEvaluation = rightCardEvaluation;
                if (rightMaxEvaluation < rightCardEvaluation) rightMaxEvaluation = rightCardEvaluation;
            }
        }
        else　//マナカーブの調整をしない時の処理
        {
            for (int i = 0; i < 2; i++)
            {
                leftTotalEvaluation += leftCardList[i].model.evaluation;
                if (leftMinEvaluation > leftCardList[i].model.evaluation) leftMinEvaluation = leftCardList[i].model.evaluation;
                if (leftMaxEvaluation < leftCardList[i].model.evaluation) leftMaxEvaluation = leftCardList[i].model.evaluation;

                rightTotalEvaluation += rightCardList[i].model.evaluation;
                if (rightMinEvaluation > rightCardList[i].model.evaluation) rightMinEvaluation = rightCardList[i].model.evaluation;
                if (rightMaxEvaluation < rightCardList[i].model.evaluation) rightMaxEvaluation = rightCardList[i].model.evaluation;
            }
        }

        float abs = leftTotalEvaluation - rightTotalEvaluation;

        //カード選択処理
        if(Mathf.Abs(abs) == 0.0)　//左右の評価値が同じときの処理
        {
            NPC1SumEvaluation++;
            NPC1RandomCount++;

            cardSelect(leftCardList, deck, NPC1deckCost);
            NPC1LeftSelectFlag = true;
            foreach (CardController card in leftCardList)
            {
                NPC1DeckEvaluation += card.model.evaluation;
            }

            /*if (leftMinEvaluation > rightMinEvaluation)　//最低評価の数値が左のほうが高い場合は左を選択
            {
                cardSelect(leftCardList, deck, NPC1deckCost);
                NPC1LeftSelectFlag = true;
                foreach (CardController card in leftCardList)
                {
                    NPC1DeckEvaluation += card.model.evaluation;
                }
            }
            else if (leftMinEvaluation < rightMinEvaluation)　//最低評価の数値が右のほうが高い場合は右を選択
            {
                cardSelect(rightCardList, deck, NPC1deckCost);
                foreach (CardController card in rightCardList)
                {
                    NPC1DeckEvaluation += card.model.evaluation;
                }
            }
            else if (leftMaxEvaluation > rightMaxEvaluation) //最低評価で決められない時は最大評価で決める
            {
                cardSelect(leftCardList, deck, NPC1deckCost);
                NPC1LeftSelectFlag = true;
                foreach (CardController card in leftCardList)
                {
                    NPC1DeckEvaluation += card.model.evaluation;
                }
            }
            else if (leftMaxEvaluation < rightMaxEvaluation)
            {
                cardSelect(rightCardList, deck, NPC1deckCost);
                foreach (CardController card in rightCardList)
                {
                    NPC1DeckEvaluation += card.model.evaluation;
                }
            }
            else //最低・最大ともに評価が同じ場合は左を選択
            {
                NPC1RandomCount++;

                cardSelect(leftCardList, deck, NPC1deckCost);
                NPC1LeftSelectFlag = true;
                foreach (CardController card in leftCardList)
                {
                    NPC1DeckEvaluation += card.model.evaluation;
                }
            }*/
        }
        else　//上記に当てはまらない場合は評価が高い方を選択
        {
            if (leftTotalEvaluation > rightTotalEvaluation)
            {
                cardSelect(leftCardList, deck, NPC1deckCost);
                NPC1LeftSelectFlag = true;
                foreach (CardController card in leftCardList)
                {
                    NPC1DeckEvaluation += card.model.evaluation;
                }
            }
            else if (leftTotalEvaluation < rightTotalEvaluation)
            {
                cardSelect(rightCardList, deck, NPC1deckCost);
                foreach (CardController card in rightCardList)
                {
                    NPC1DeckEvaluation += card.model.evaluation;
                }
            }
        }
    }

    //評価が高い選択肢をただ選ぶNPC
    void NPC2Pick(Deck deck)
    {
        //左側の選択肢
        float leftTotalEvaluation = 0f; //選択肢の評価合計値

        //右側の選択肢
        float rightTotalEvaluation = 0f;

        //左右の評価を計算
        for (int i = 0; i < 2; i++)
        {
            leftTotalEvaluation += leftCardList[i].model.evaluation;
            rightTotalEvaluation += rightCardList[i].model.evaluation;
        }

        float abs = leftTotalEvaluation - rightTotalEvaluation;

        //カード選択処理
        if (Mathf.Abs(abs) == 0.0)　//左右の評価値が同じときの処理
        {
            NPC2SumEvaluation++;
            NPC2RandomCount++;

            cardSelect(leftCardList, deck, NPC2deckCost);
            NPC2LeftSelectFlag = true;
        }
        else　//上記に当てはまらない場合は評価が高い方を選択
        {
            if (leftTotalEvaluation > rightTotalEvaluation)
            {
                cardSelect(leftCardList, deck, NPC2deckCost);
                NPC2LeftSelectFlag = true;
            }
            else if (leftTotalEvaluation < rightTotalEvaluation)
            {
                cardSelect(rightCardList, deck, NPC2deckCost);
            }
        }
    }

    //マナカーブ実験用
    float EvaluationFluctuation(CardController card, int[] deckManaCost)
    {
        float fluctuationNum = 0;

        //マナカーブ確認処理
        switch (card.model.cost)
        {
            case 1: //1コスト
                if (deckManaCost[0] >= 3)
                {
                    fluctuationNum = card.model.evaluation - evaluationFluctuation;
                }
                else
                {
                    fluctuationNum = card.model.evaluation;
                }
                break;
            case 2: //2コスト
                if (deckManaCost[1] < 6)
                {
                    fluctuationNum = card.model.evaluation + evaluationFluctuation;
                }
                else if (deckManaCost[1] >= 9)
                {
                    fluctuationNum = card.model.evaluation - evaluationFluctuation;
                }
                else
                {
                    fluctuationNum = card.model.evaluation;
                }
                break;
            case 3: //3コスト
                if (deckManaCost[2] < 4)
                {
                    fluctuationNum = card.model.evaluation + evaluationFluctuation;
                }
                else if (deckManaCost[2] >= 6)
                {
                    fluctuationNum = card.model.evaluation - evaluationFluctuation;
                }
                else
                {
                    fluctuationNum = card.model.evaluation;
                }
                break;
            case 4: //4コスト
                if (deckManaCost[3] < 3)
                {
                    fluctuationNum = card.model.evaluation + evaluationFluctuation;
                }
                else if (deckManaCost[3] >= 6)
                {
                    fluctuationNum = card.model.evaluation - evaluationFluctuation;
                }
                else
                {
                    fluctuationNum = card.model.evaluation;
                }
                break;
            case 5: //5コスト
                if (deckManaCost[4] < 2)
                {
                    fluctuationNum = card.model.evaluation + evaluationFluctuation;
                }
                else if (deckManaCost[4] >= 4)
                {
                    fluctuationNum = card.model.evaluation - evaluationFluctuation;
                }
                else
                {
                    fluctuationNum = card.model.evaluation;
                }
                break;
            case 6: //6コスト
                if (deckManaCost[5] < 1)
                {
                    fluctuationNum = card.model.evaluation + evaluationFluctuation;
                }
                else if (deckManaCost[5] >= 4)
                {
                    fluctuationNum = card.model.evaluation - evaluationFluctuation;
                }
                else
                {
                    fluctuationNum = card.model.evaluation;
                }
                break;
            case 7: //7コスト
                if (deckManaCost[6] < 1)
                {
                    fluctuationNum = card.model.evaluation + evaluationFluctuation;
                }
                else if (deckManaCost[6] >= 4)
                {
                    fluctuationNum = card.model.evaluation - evaluationFluctuation;
                }
                else
                {
                    fluctuationNum = card.model.evaluation;
                }
                break;
            default: //8コスト以上
                if (deckManaCost[7] < 1)
                {
                    fluctuationNum = card.model.evaluation + evaluationFluctuation;
                }
                else if (deckManaCost[7] >= 4)
                {
                    fluctuationNum = card.model.evaluation - evaluationFluctuation;
                }
                else
                {
                    fluctuationNum = card.model.evaluation;
                }
                break;
        }

        return fluctuationNum;
    }

    //デッキ評価
    void ManaCarveEvaluation(int[] manaCarve)
    {
        int deckEvalution = 0;

        if(manaCarve[0] >=0 && manaCarve[0] <=3)
        {
            deckEvalution++;
        }

        if (manaCarve[1] >= 6 && manaCarve[1] <= 9)
        {
            deckEvalution++;
        }

        if (manaCarve[2] >= 4 && manaCarve[2] <= 6)
        {
            deckEvalution++;
        }

        if (manaCarve[3] >= 3 && manaCarve[3] <= 6)
        {
            deckEvalution++;
        }

        if (manaCarve[4] >= 2 && manaCarve[4] <= 4)
        {
            deckEvalution++;
        }

        for (int i = 5; i < 8;i++)
        {
            if (manaCarve[i] >= 1 && manaCarve[i] <= 4)
            {
                deckEvalution++;
            }
        }

        Debug.Log(deckEvalution);
    }
}
