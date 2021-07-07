using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectEvent : MonoBehaviour
{
    [SerializeField] GameObject cardEdge;
    //DraftManegerを探す
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
        //カード情報取得
        CardController selectCard = GetComponent<CardController>();

        //表示リセット処理
        draftManagerScript.ResetField();

        //デッキ保存処理
        draftManagerScript.cardSelect(selectCard);

        if (draftManagerScript.selectEnd == false)
        {
            //カード再生成処理
            draftManagerScript.CreateDraftCard();
        }
    }
}
