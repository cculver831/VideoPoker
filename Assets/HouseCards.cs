using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Deck of Cards for Player o use
public class HouseCards : MonoBehaviour
{
    [SerializeField]
    List<Card> Deck;

    [SerializeField]
    GameObject Player;
    private int Top = 0;
    private void Start()
    {
        Shuffle();
        DealCards();
    }

    private void Deal(CardObject c)
    {
        //Deal card from top of the deck
        Card newCard = RemoveFromTop();
        c.myCard = newCard;
        Debug.Log("Card Dealt: "+ newCard.name);
        c.UpdateImage();
    }

    public Card RemoveFromTop()
    {
        //Remove card from top of deck
        Card TopCard = Deck[Top];
        //check card can be played
        if (!TopCard.isPlayed)
        {
            RemoveCard(Top);
        }
       
        if (Top >= 51)
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
        //Debug.Log("Grabbing Cards");
        List<GameObject> Hand = Player.GetComponent<PlayerCards>().ShowHand();
        Deal(Hand);
    }

    private void Deal(List<GameObject> cards)
    {
        List<Card> newCards = new List<Card>();
        //loop to assign cards
        for (int i = 0; i < cards.Count; i++)
        {
            CardObject c = cards[i].GetComponent<CardObject>();
            if (!c.Hold)
            {
                //Debug.Log("Dealing new Card: ");
                Deal(c);
            }
               
        }

    }

    //Shuffle Deck
     void Shuffle()
    {
        for (var i = 0; i < Deck.Count; i++)
        {
            int newPlace = Random.Range(i, Deck.Count);
            Card temp = Deck[i];
            Deck[i] = Deck[newPlace];
            Deck[newPlace] = temp;
        }
    }

    //Remove Card from deck
    void RemoveCard(int index)
    {
        Card c = Deck[index];
        c.isPlayed = true;
    }
}
