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

    //�I���I���t���O
    [System.NonSerialized] public bool selectEnd;

    [SerializeField] bool NPCDeckBuildFlag;
    int NPCChangeEvaluation;

    int Cost2Count;

    int end = 12; //�J�[�h�̎�ނ��擾�i���̂����������������j
    
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

                //�f�b�L���ċN������ۑ����鏈��
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
                //�J�[�h�Đ�������
                CreateDraftCard();
            }
        }
    }

    //NPC�̎v�l����
    //2�R�X�̖�������������NPC
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

    //�����_���ȃJ�[�h���s�b�N����NPC
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
