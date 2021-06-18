using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectEvent : MonoBehaviour
{
    public void MyPointerDownUI()
    {
        //ここにデッキ保存のためのスクリプトを追加する

        //カード再生成処理
        GameObject draftManagerObject = GameObject.Find("DraftManager") as GameObject;
        DraftManager draftManagerScript = draftManagerObject.GetComponent<DraftManager>();
        draftManagerScript.ResetField();
        draftManagerScript.CreateDraftCard();
    }
}
