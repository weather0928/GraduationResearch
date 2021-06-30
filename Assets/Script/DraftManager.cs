using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab; //�J�[�h��prefab�擾
    [SerializeField] Transform cardSelectField; //�J�[�h��\������ꏊ���擾
    [SerializeField] int deckCount; //�f�b�L�̖���

    //�I�𕔕��ɏo�Ă��Ă���J�[�h�̏���ێ��i���Z�b�g�p�j
    List<CardController> fieldCardList = new List<CardController>();

    //�I�������J�[�h�̏���ێ�
    List<CardController> deckList = new List<CardController>();

    int end = 5; //�J�[�h�̎�ނ��擾�i���̂����������������j
    
    void Start()
    {
        if(deckCount < 1)
        {
            deckCount = 1;
        }
        CreateDraftCard();
    }

    //�J�[�h��������
    public void CreateDraftCard()
    {
        List<int> cardIDList = new List<int>();

        for (int i = 1; i <= end; i++)
        {
            cardIDList.Add(i);
        }

        int count = 3;

        while (count-- > 0)
        {
            int index = Random.Range(0, cardIDList.Count);
            int cardID = cardIDList[index];
            CardController card = Instantiate(cardPrefab, cardSelectField);
            card.Init(cardID);
            fieldCardList.Add(card);
            cardIDList.RemoveAt(index);
        }
    }

    //�I���J�[�h�ۑ�����
    public void cardSelect(CardController selectCard)
    {
        deckList.Add(selectCard);
        Debug.Log(deckList[deckList.Count - 1].model.name);//�e�X�g�p
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

    //�J�[�h�����@�\�i���͏�Ɠ����B�����̕K�v���������炱�����g���j
    /*void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, CardSelectField);
        card.Init(cardID);
    }*/
}
