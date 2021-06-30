using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectEvent : MonoBehaviour
{
    public void MyPointerDownUI()
    {
        //DraftManeger��T��
        GameObject draftManagerObject = GameObject.Find("DraftManager") as GameObject;
        DraftManager draftManagerScript = draftManagerObject.GetComponent<DraftManager>();

        //�J�[�h���擾
        CardController selectCard = GetComponent<CardController>();

        //�����Ƀf�b�L�ۑ��̂��߂̃X�N���v�g��ǉ�����
        draftManagerScript.cardSelect(selectCard);

        //�J�[�h�Đ�������
        draftManagerScript.ResetField();
        draftManagerScript.CreateDraftCard();
    }
}
