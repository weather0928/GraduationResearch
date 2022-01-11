using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectEvent : MonoBehaviour
{

    [SerializeField] bool rightButton;
    [SerializeField] bool leftButton;
    //DraftManeger��T��
    GameObject draftManagerObject;
    DraftManager draftManagerScript;

    Deck deck;

    private void Start()
    {
        draftManagerObject = GameObject.Find("DraftManager") as GameObject;
        draftManagerScript = draftManagerObject.GetComponent<DraftManager>();
        deck = Resources.Load<Deck>("Deck/Test");
    }

    public void MyPointerDownUI()
    {

        //�f�b�L�ۑ�����
        if (rightButton == true)
        {
            draftManagerScript.cardSelect(draftManagerScript.rightCardList,deck,draftManagerScript.playerDeckCost);
        }
        else if (leftButton == true)
        {
            draftManagerScript.cardSelect(draftManagerScript.leftCardList,deck, draftManagerScript.playerDeckCost);
        }

        //�\�����Z�b�g����
        draftManagerScript.ResetField();

        if (draftManagerScript.selectEnd == false)
        {
            //�J�[�h�Đ�������
            draftManagerScript.CreateDraftCard();
        }
    }
}
