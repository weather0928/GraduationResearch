using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class DraftManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab; //�J�[�h��prefab�擾

    [SerializeField] GameObject draftCanvas;
    [SerializeField] Transform cardSelectFieldR; //�J�[�h��\������ꏊ���擾�i�E���j
    [SerializeField] Transform cardSelectFieldL; //�J�[�h��\������ꏊ���擾�i�����j

    [SerializeField] GameObject deckCanvas;
    [SerializeField] Transform cardDisplayCanvas;

    [SerializeField] int pickCount; //�s�b�N��
    int deckCount; //�f�b�L�̖����i�s�b�N�񐔁~�Q�j

    //�����ɏo�Ă��Ă���J�[�h�̏���ێ�
    [System.NonSerialized] public List<CardController> leftCardList = new List<CardController>();

    //�E���ɏo�Ă��Ă���J�[�h�̏���ێ�
    [System.NonSerialized] public List<CardController> rightCardList = new List<CardController>();

    //�f�b�L���
    Deck playerDeck;
    Deck NPCDeck1, NPCDeck2;
    [System.NonSerialized] public int[] playerDeckCost = new int[8]; //�}�i�J�[�u���m�F���邽�߂̂���
    int[] NPC1deckCost = new int[8]; //�}�i�J�[�u���m�F���邽�߂̂���
    int[] NPC2deckCost = new int[8]; //�}�i�J�[�u���m�F���邽�߂̂���

    //NPC�f�b�L����p�B�ǂꂭ�炢�]����ϓ�������̂�������
    [SerializeField] int evaluationFluctuation;

    //�I���I���t���O
    [System.NonSerialized] public bool selectEnd;

    [SerializeField] bool NPCDeckBuildFlag;
    int NPCChangeEvaluation;

    int Cost2Count;

    int end = 63; //�J�[�h�̎�ނ��擾�i���̂����������������j
    
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

    //�J�[�h��������
    public void CreateDraftCard()
    {
        //�J�[�h���X�g������
        List<int> cardIDList = new List<int>();

        for (int i = 1; i <= end; i++)
        {
            cardIDList.Add(i);
        }

        //�J�[�h�����E�ɕ\��
        for (int i = 0; i < 2; i++)
        {
            int index, cardID;

            //����
            index = Random.Range(0, cardIDList.Count);
            cardID = cardIDList[index];
            CardController leftCard = CreatePickCard(cardID, cardSelectFieldL);
            leftCardList.Add(leftCard);
            cardIDList.RemoveAt(index);

            //�E��
            index = Random.Range(0, cardIDList.Count);
            cardID = cardIDList[index];
            CardController rightCard = CreatePickCard(cardID, cardSelectFieldR);
            rightCardList.Add(rightCard);
            cardIDList.RemoveAt(index);
        }
    }

    //�I���J�[�h�ۑ�����
    public void cardSelect(List<CardController> cardList,Deck deck,int[] manaCost)
    {
        if(selectEnd == false)
        { 
            for (int i = 0; i < cardList.Count; i++)
            {
                deck.cardList.Add(cardList[i].model.cardID);

                //�f�b�L���}�i�R�X�g���f����
                if(cardList[i].model.cost < 8)
                {
                    manaCost[cardList[i].model.cost - 1]++;
                }
                else
                {
                    manaCost[7]++;
                }
            }

            //�f�b�L��������
            //�v���C���[���f�b�L�����ꍇ
            if(NPCDeckBuildFlag == false)
            {
                if (deck.cardList.Count == deckCount)
                {
                    draftCanvas.SetActive(false);
                    deckCanvas.SetActive(true);
                    selectEnd = true;

                    //�f�b�L���ċN������ۑ����鏈��
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

    //�I����ʃ��Z�b�g����
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

    //�J�[�h�����@�\�i�s�b�N��ʗp�j
    CardController CreatePickCard(int cardID,Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(cardID);
        return card;
    }

    //�J�[�h�����@�\�i�f�b�L�\���p�j
    void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(cardID);
    }

    //NPC�f�b�L���쏈��(���ʕ���)
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

                //�f�b�L���ċN������ۑ����鏈��
                EditorUtility.SetDirty(NPCDeck1);
                //EditorUtility.SetDirty(NPCDeck2);
                AssetDatabase.SaveAssets();

                selectEnd = true;
            }

            ResetField();

            if (selectEnd == false)
            {
                //�J�[�h�Đ�������
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

    //NPC�̎v�l����
    //�}�i�J�[�u�̒���������NPC
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

    //�����_���ȃJ�[�h���s�b�N����NPC
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

    //�}�i�J�[�u�����p
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

    //�f�b�L�]��
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
