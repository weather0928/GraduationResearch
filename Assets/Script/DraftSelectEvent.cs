using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectEvent : MonoBehaviour
{
    public void MyPointerDownUI()
    {
        //DraftManegerを探す
        GameObject draftManagerObject = GameObject.Find("DraftManager") as GameObject;
        DraftManager draftManagerScript = draftManagerObject.GetComponent<DraftManager>();

        //カード情報取得
        CardController selectCard = GetComponent<CardController>();

        //ここにデッキ保存のためのスクリプトを追加する
        draftManagerScript.cardSelect(selectCard);

        //カード再生成処理
        draftManagerScript.ResetField();
        draftManagerScript.CreateDraftCard();
    }
}
