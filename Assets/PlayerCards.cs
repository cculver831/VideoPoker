using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCards : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> PlayerHand;

    public int balance = 500;
    // Start is called before the first frame update
    void Start()
    {
        // EventManager.current.onPlayerBet += Bet1;
    }


    public List<GameObject> ShowHand()
    {
        return PlayerHand;
    }

    public List<Card> ShowCards()
    {
        List<Card> myCards = new List<Card>();
        foreach (GameObject card in PlayerHand)
        {
            myCards.Add(card.GetComponent<CardObject>().myCard);
        }
        return myCards;
    }

    public int GetBalance()
    {
        return balance;
    }

    public void SetBalance(int newBalance)
    {
        balance += newBalance;
    }

    public int[] GetValues()
    {
        int[] Values = new int[5];
        for (int i = 0; i < PlayerHand.Count; i++)
        {
            GameObject c = PlayerHand[i];
            int cardVal = c.GetComponent<CardObject>().myCard.Value;
            Values[i] = cardVal;
        }

        return Values;
    }

    public void ResetHolds()
    {
        foreach( GameObject c in PlayerHand)
        {
            CardObject card = c.GetComponent<CardObject>();
            card.Hold = false;
            card.UpdateHold();
        }
    }
}
