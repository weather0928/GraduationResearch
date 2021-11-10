using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectEvent : MonoBehaviour
{

    [SerializeField] bool rightButton;
    [SerializeField] bool leftButton;
    //DraftManegerを探す
    GameObject draftManagerObject;
    DraftManager draftManagerScript;

    private void Start()
    {
        draftManagerObject = GameObject.Find("DraftManager") as GameObject;
        draftManagerScript = draftManagerObject.GetComponent<DraftManager>();
    }

    public void MyPointerDownUI()
    {
        //カード情報取得
        CardController selectButton = GetComponent<CardController>();

        //デッキ保存処理
        if (rightButton == true)
        {
            draftManagerScript.cardSelect(2,4);
        }
        else if (leftButton == true)
        {
            draftManagerScript.cardSelect(0,2);
        }

        //表示リセット処理
        draftManagerScript.ResetField();

        if (draftManagerScript.selectEnd == false)
        {
            //カード再生成処理
            draftManagerScript.CreateDraftCard();
        }
    }
}
