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

    Deck deck;

    private void Start()
    {
        draftManagerObject = GameObject.Find("DraftManager") as GameObject;
        draftManagerScript = draftManagerObject.GetComponent<DraftManager>();
        deck = Resources.Load<Deck>("Deck/Test");
    }

    public void MyPointerDownUI()
    {

        //デッキ保存処理
        if (rightButton == true)
        {
            draftManagerScript.cardSelect(draftManagerScript.rightCardList,deck);
        }
        else if (leftButton == true)
        {
            draftManagerScript.cardSelect(draftManagerScript.leftCardList,deck);
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
