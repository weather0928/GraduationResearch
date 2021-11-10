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

    private void Start()
    {
        draftManagerObject = GameObject.Find("DraftManager") as GameObject;
        draftManagerScript = draftManagerObject.GetComponent<DraftManager>();
    }

    public void MyPointerDownUI()
    {
        //�J�[�h���擾
        CardController selectButton = GetComponent<CardController>();

        //�f�b�L�ۑ�����
        if (rightButton == true)
        {
            draftManagerScript.cardSelect(2,4);
        }
        else if (leftButton == true)
        {
            draftManagerScript.cardSelect(0,2);
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
