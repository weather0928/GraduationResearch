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
        /*for(int i = 0;i < 3;i++)
        {
            randomCardID[i] = Random.Range(0, 3);
        }*/
        StartGame();
    }

    void StartGame()
    {
        CreateCard(1, CardSelectField);
        CreateCard(2, CardSelectField);
        CreateCard(3, CardSelectField);
        /*for(int i = 0;i < 3;i++)
        {
            CreateCard(1, CardSelectField);
        }*/
    }

    void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, CardSelectField);
        card.Init(cardID);
    }
}
