using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectEvent : MonoBehaviour
{
    [SerializeField] GameObject cardEdge;
    //DraftManeger��T��
    GameObject draftManagerObject;
    DraftManager draftManagerScript;

    private void Start()
    {
        cardEdge.SetActive(false);
        draftManagerObject = GameObject.Find("DraftManager") as GameObject;
        draftManagerScript = draftManagerObject.GetComponent<DraftManager>();
    }

    public void MyPointerEnterUI()
    {
        if (draftManagerScript.selectEnd == false)
        {
            cardEdge.SetActive(true);
        }
    }

    public void MyPointerExitUI()
    {
        if (draftManagerScript.selectEnd == false)
        {
            cardEdge.SetActive(false);
        }
    }

    public void MyPointerDownUI()
    {
        //�J�[�h���擾
        CardController selectCard = GetComponent<CardController>();

        //�\�����Z�b�g����
        draftManagerScript.ResetField();

        //�f�b�L�ۑ�����
        draftManagerScript.cardSelect(selectCard);

        if (draftManagerScript.selectEnd == false)
        {
            //�J�[�h�Đ�������
            draftManagerScript.CreateDraftCard();
        }
    }
}
