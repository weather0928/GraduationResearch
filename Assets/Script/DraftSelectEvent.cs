using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectEvent : MonoBehaviour
{
    public void MyPointerDownUI()
    {
        //�����Ƀf�b�L�ۑ��̂��߂̃X�N���v�g��ǉ�����

        //�J�[�h�Đ�������
        GameObject draftManagerObject = GameObject.Find("DraftManager") as GameObject;
        DraftManager draftManagerScript = draftManagerObject.GetComponent<DraftManager>();
        draftManagerScript.ResetField();
        draftManagerScript.CreateDraftCard();
    }
}
