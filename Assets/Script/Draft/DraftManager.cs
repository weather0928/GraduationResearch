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

    int end = 63; //�J�[�h�̎�ނ��擾�i���̂����������������j

    //�����p�ϐ�
    //���ꂼ��̑I�����̕]�����߂����Ɍv������ϐ��Q
    bool NPC1LeftSelectFlag = false; //NPC1�����̑I������I�񂾎���true�ɂȂ�
    bool NPC2LeftSelectFlag = false; //NPC2�����̑I������I�񂾎���true�ɂȂ�
    int NPCDifSelect = 0; //2��NPC���ʁX�̑I�������s�����񐔂��L�^
    int NPC1RandomCount = 0; //NPC1�������_�����g������
    int NPC2RandomCount = 0; //NPC2�������_�����g������
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
            NPC1Pick(NPCDeck1);
            NPC2Pick(NPCDeck2);

            //�����p
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

                //�f�b�L���ċN������ۑ����鏈��
                EditorUtility.SetDirty(NPCDeck1);
                EditorUtility.SetDirty(NPCDeck2);
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

    //NPC�̎v�l����
    //��Ď�@�����g�ݍ��킹��NPC
    void NPC1Pick(Deck deck)
    {
        //�����̑I����
        float leftMinEvaluation = 100f; //�Œ�]���̃J�[�h���v��
        float leftMaxEvaluation = -100f; //�ő�]���̃J�[�h���v��
        float leftTotalEvaluation = 0f; //�I�����̕]�����v�l

        //�E���̑I����
        float rightMinEvaluation = 100f;
        float rightMaxEvaluation = -100f;
        float rightTotalEvaluation = 0f;

        if (NPCDeck1.cardList.Count >= NPCChangeEvaluation)�@//�}�i�J�[�u�̒����̂��߂ɕ]����ς��鏈��
        {
            for (int i = 0;i < 2;i++)
            {
                //�J�[�h�̕]����ϓ��������l��ۑ�����
                float leftCardEvaluation; //�����̃J�[�h
                float rightCardEvaluation; //�E���̃J�[�h

                //�]����ϓ������鏈��
                //����
                leftCardEvaluation = EvaluationFluctuation(leftCardList[i], NPC1deckCost);
                leftTotalEvaluation += leftCardEvaluation;
                if (leftMinEvaluation > leftCardEvaluation) leftMinEvaluation = leftCardEvaluation;
                if (leftMaxEvaluation < leftCardEvaluation) leftMaxEvaluation = leftCardEvaluation;

                //�E��
                rightCardEvaluation = EvaluationFluctuation(rightCardList[i], NPC1deckCost);
                rightTotalEvaluation += rightCardEvaluation;
                if (rightMinEvaluation > rightCardEvaluation) rightMinEvaluation = rightCardEvaluation;
                if (rightMaxEvaluation < rightCardEvaluation) rightMaxEvaluation = rightCardEvaluation;
            }
        }
        else�@//�}�i�J�[�u�̒��������Ȃ����̏���
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

        //�J�[�h�I������
        if(Mathf.Abs(abs) == 0.0)�@//���E�̕]���l�������Ƃ��̏���
        {
            NPC1SumEvaluation++;
            NPC1RandomCount++;

            cardSelect(leftCardList, deck, NPC1deckCost);
            NPC1LeftSelectFlag = true;
            foreach (CardController card in leftCardList)
            {
                NPC1DeckEvaluation += card.model.evaluation;
            }

            /*if (leftMinEvaluation > rightMinEvaluation)�@//�Œ�]���̐��l�����̂ق��������ꍇ�͍���I��
            {
                cardSelect(leftCardList, deck, NPC1deckCost);
                NPC1LeftSelectFlag = true;
                foreach (CardController card in leftCardList)
                {
                    NPC1DeckEvaluation += card.model.evaluation;
                }
            }
            else if (leftMinEvaluation < rightMinEvaluation)�@//�Œ�]���̐��l���E�̂ق��������ꍇ�͉E��I��
            {
                cardSelect(rightCardList, deck, NPC1deckCost);
                foreach (CardController card in rightCardList)
                {
                    NPC1DeckEvaluation += card.model.evaluation;
                }
            }
            else if (leftMaxEvaluation > rightMaxEvaluation) //�Œ�]���Ō��߂��Ȃ����͍ő�]���Ō��߂�
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
            else //�Œ�E�ő�Ƃ��ɕ]���������ꍇ�͍���I��
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
        else�@//��L�ɓ��Ă͂܂�Ȃ��ꍇ�͕]������������I��
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

    //�]���������I�����������I��NPC
    void NPC2Pick(Deck deck)
    {
        //�����̑I����
        float leftTotalEvaluation = 0f; //�I�����̕]�����v�l

        //�E���̑I����
        float rightTotalEvaluation = 0f;

        //���E�̕]�����v�Z
        for (int i = 0; i < 2; i++)
        {
            leftTotalEvaluation += leftCardList[i].model.evaluation;
            rightTotalEvaluation += rightCardList[i].model.evaluation;
        }

        float abs = leftTotalEvaluation - rightTotalEvaluation;

        //�J�[�h�I������
        if (Mathf.Abs(abs) == 0.0)�@//���E�̕]���l�������Ƃ��̏���
        {
            NPC2SumEvaluation++;
            NPC2RandomCount++;

            cardSelect(leftCardList, deck, NPC2deckCost);
            NPC2LeftSelectFlag = true;
        }
        else�@//��L�ɓ��Ă͂܂�Ȃ��ꍇ�͕]������������I��
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

    //�}�i�J�[�u�����p
    float EvaluationFluctuation(CardController card, int[] deckManaCost)
    {
        float fluctuationNum = 0;

        //�}�i�J�[�u�m�F����
        switch (card.model.cost)
        {
            case 1: //1�R�X�g
                if (deckManaCost[0] >= 3)
                {
                    fluctuationNum = card.model.evaluation - evaluationFluctuation;
                }
                else
                {
                    fluctuationNum = card.model.evaluation;
                }
                break;
            case 2: //2�R�X�g
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
            case 3: //3�R�X�g
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
            case 4: //4�R�X�g
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
            case 5: //5�R�X�g
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
            case 6: //6�R�X�g
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
            case 7: //7�R�X�g
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
            default: //8�R�X�g�ȏ�
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

    //�f�b�L�]��
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
