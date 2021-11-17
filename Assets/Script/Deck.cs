using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Deck",fileName = "Deck")]
public class Deck : ScriptableObject
{
    public List<int> cardList;
}
