using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab; //�J�[�h��prefab�擾
    [SerializeField] Transform cardSelectFieldR; //�J�[�h��\������ꏊ���擾�i�E���j
    [SerializeField] Transform cardSelectFieldL; //�J�[�h��\������ꏊ���擾�i�����j
    [SerializeField] Text text; //��ʏ㕔�̕\������
    [SerializeField] int pickCount; //�s�b�N��
    int deckCount; //�f�b�L�̖����i�s�b�N�񐔁~�Q�j

    //�I�𕔕��ɏo�Ă��Ă���J�[�h�̏���ێ��i���Z�b�g�p�j
    List<CardController> fieldCardList = new List<CardController>();

    //�I�𕔕��ɏo�Ă��Ă���J�[�h��ID��ێ�
    List<int> fieldCardIDList = new List<int>();

    //�I�������J�[�h�̏���ێ�
    List<CardController> deckList = new List<CardController>();

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
        text.text = "�ǂ��炩�̃J�[�h��I�����Ă�������";
        selectEnd = false;
        CreateDraftCard();
    }

    //�J�[�h��������
    public void CreateDraftCard()
    {
        //�I�����̐�
        int choices = 2;

        //�I�΂ꂽ�J�[�h��ID��ۑ�
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

        //�J�[�h�����E�ɕ\���i2�����j
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

    //�I���J�[�h�ۑ�����
    public void cardSelect(CardController selectCard)
    {
        if(selectEnd == false)
        {
            deckList.Add(selectCard);

            //�f�b�L��������
            if (deckList.Count == deckCount)
            {
                selectEnd = true;
                text.text = "�f�b�L���e";
                /*for (int i = 0; i < deckList.Count; i++)
                {
                    CreateCard(deckList[i].model.cardID, cardSelectField);
                }*/
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
    /*void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, cardSelectField);
        card.Init(cardID);
    }*/
}
