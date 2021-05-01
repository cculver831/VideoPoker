using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Deck of Cards for Player o use
public class HouseCards : MonoBehaviour
{
    [SerializeField]
    Card[] Deck;

    [SerializeField]
    GameObject Player;
    private int Top = 0;
    private void Start()
    {
        
    }
    public void Shuffle()
    {
        //shuffle cards
    }

    private Card Deal(CardObject c)
    {
        //Deal card from top of the deck
        Card newCard = RemoveFromTop();
        c.myCard = newCard;

        return newCard;
    }

    public Card RemoveFromTop()
    {
        //Remove card from top of deck
        Card TopCard = Deck[0];
        if (Top >= 53)
            Top = 0;
        else
        {
            Top++;
        }
        return TopCard;
    }


    //
    public void DealCards()
    {
        List<GameObject> Hand = Player.GetComponent<PlayerCards>().ShowHand();
        Deal(Hand);
    }
    
    private void Deal(List<GameObject> cards)
    {
        List<Card> newCards = new List<Card>();
        //loop to assign cards
        for(int i = 0; i < cards.Count; i++)
        {
            CardObject c = cards[i].GetComponent<CardObject>();
            if(c.Hold)
                newCards.Add(Deal(c));
        }

       // return newCards;
    }
}
