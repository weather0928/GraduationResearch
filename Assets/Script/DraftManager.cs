using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //�I�𕔕��ɏo�Ă��Ă���J�[�h�̏���ێ��i���Z�b�g�p�j
    List<CardController> fieldCardList = new List<CardController>();

    //�I�𕔕��ɏo�Ă��Ă���J�[�h��ID��ێ�
    [System.NonSerialized]public List<int> fieldCardIDList = new List<int>();

    //�I�������J�[�h�̏���ێ�
    List<CardController> deckList = new List<CardController>();

    //�I���I���t���O
    [System.NonSerialized] public bool selectEnd;

    [SerializeField] bool NPCDeckBuildFlag;
    int NPCChangeEvaluation;

    int Cost2Count;

    int end = 10; //�J�[�h�̎�ނ��擾�i���̂����������������j
    
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
            NPCChangeEvaluation = deckCount / 3 * 2;
            NPCDeckBuild();
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

        int cardCount = 4;

        //�J�[�h�I�o����
        while (cardCount-- > 0)
        {
            int index = Random.Range(0, cardIDList.Count);
            int cardID = cardIDList[index];
            fieldCardIDList.Add(cardID);
            cardIDList.RemoveAt(index);
        }

        //�J�[�h�����E�ɕ\��
        //����
        for (int i = 0; i < 2; i++)
        {
            CardController leftCard = CreatePickCard(fieldCardIDList[i]
                , cardSelectFieldL);
            fieldCardList.Add(leftCard);
        }

        for (int i = 2;i < 4;i++)
        {
            //�E��
            CardController rightCard = CreatePickCard(fieldCardIDList[i]
                , cardSelectFieldR);
            fieldCardList.Add(rightCard);
        }
    }

    //�I���J�[�h�ۑ�����
    public void cardSelect(int start,int end)
    {
        if(selectEnd == false)
        { 
            for (int i = start; i < end; i++)
            {
                deckList.Add(fieldCardList[i]);
                if(fieldCardList[i].model.cost == 2)
                {
                    Cost2Count++;
                }
            }

            fieldCardIDList.Clear();

            //�f�b�L��������
            if (deckList.Count == deckCount)
            {
                draftCanvas.SetActive(false);
                deckCanvas.SetActive(true);
                selectEnd = true;
                for (int i = 0; i < deckList.Count; i++)
                {
                    CreateCard(deckList[i].model.cardID, cardDisplayCanvas);
                }
                Debug.Log(Cost2Count);
            }
        }
    }

    //�I����ʃ��Z�b�g����
    public void ResetField()
    {
        for(int i = 0;i < fieldCardList.Count;i++)
        {
            Destroy(fieldCardList[i].gameObject);
        }
        fieldCardList.Clear();
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

    //NPC�f�b�L���쏈��
    void NPCDeckBuild()
    {
        while(selectEnd == false)
        {
            int leftSelectEvaluation;
            int rightSelectEvaluation;

            leftSelectEvaluation = fieldCardList[0].model.evaluation
                + fieldCardList[1].model.evaluation;
            rightSelectEvaluation = fieldCardList[2].model.evaluation
                + fieldCardList[3].model.evaluation;

            if(deckList.Count >= NPCChangeEvaluation 
                && Cost2Count < deckCount / 3)
            {
                Debug.Log("aaa");
                for(int i = 0;i < 2;i++)
                {
                    if(fieldCardList[i].model.cost == 2)
                    {
                        leftSelectEvaluation += 5;
                    }
                    if (fieldCardList[i + 2].model.cost == 2)
                    {
                        rightSelectEvaluation += 5;
                    }
                }
            }

            if(leftSelectEvaluation >= rightSelectEvaluation)
            {
                cardSelect(0, 2);
            }
            else if (leftSelectEvaluation < rightSelectEvaluation)
            {
                cardSelect(2, 4);
            }

            ResetField();

            if (selectEnd == false)
            {
                //�J�[�h�Đ�������
                CreateDraftCard();
            }
        }
    }
}
