using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab;
    [SerializeField] Transform CardSelectField;
    int[] randomCardID = new int[3];

    void Start()
    {
        for(int i = 0;i < 3;i++)
        {
            randomCardID[i] = Random.Range(1, 4);
        }
        StartGame();
    }

    void StartGame()
    {
        for(int i = 0;i < 3;i++)
        {
            CreateCard(randomCardID[i], CardSelectField);
        }
    }

    void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, CardSelectField);
        card.Init(cardID);
    }
}
