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
    List<int> fieldCardIDList = new List<int>();

    //�I�������J�[�h��ID��ێ�
    List<int> deckList = new List<int>();

    //�I���I���t���O
    [System.NonSerialized] public bool selectEnd;

    int end = 5; //�J�[�h�̎�ނ��擾�i���̂����������������j
    
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
        for (int i = 0; i < 2; i++)
        {
            //����
            CardController leftCard = Instantiate(cardPrefab, cardSelectFieldL);
            leftCard.Init(fieldCardIDList[i]);
            fieldCardList.Add(leftCard);

            //�E��
            CardController rightCard = Instantiate(cardPrefab, cardSelectFieldR);
            rightCard.Init(fieldCardIDList[i + 2]);
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
                deckList.Add(fieldCardIDList[i]);
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
                    CreateCard(deckList[i], cardDisplayCanvas);
                }
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

    //�J�[�h�����@�\�i�f�b�L�\���p�j
    void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(cardID);
    }
}
