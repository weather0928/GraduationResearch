using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] Text nameText, powerText, hpText,costText;
    //[SerializeField] Image iconImage;

    public void Show(CardModel cardModel)
    {
        nameText.text = cardModel.name;
        costText.text = cardModel.cost.ToString();
        powerText.text = cardModel.power.ToString();
        hpText.text = cardModel.hp.ToString();
    }
}
