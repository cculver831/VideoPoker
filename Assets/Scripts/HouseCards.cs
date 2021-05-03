using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Deck of Cards for Player to use
public class HouseCards : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField]
    Sprite CardDown;
    [SerializeField]
    AudioClip CardFlip;
    [SerializeField]
    AudioSource Source;


    [Header("Deck")]
    [SerializeField]
    List<Card> Deck;

    [Header("Player Ref")]
    [SerializeField]
    PlayerCards Player;
    

    [Header("UI Ref")]
    [SerializeField]
    GameObject PlayAgainButton;
    [SerializeField]
    GameObject DealButton;
    [SerializeField]
    Text CurrentBet;
    [SerializeField]
    Text Balance;
    [SerializeField]
    Text WinType;
    

    //  Card Reading/Dealing 
    string[] straights = new string[10] { "234514", "23456", "34567", "45678", "56789", "678910", "7891011", "89101112", "910111213", "1011121314" };
    string RoyalStraight = "1011121314";
    string[] Wins = new string[9] { "Royal Flush", "Straight Flush", "4 of a kind", "Full House", "Flush", "Straight", "3 of a kind", "Two Pair", "Jacks or Better" };
    int[] PayoutDefault = new int[9] { 250, 50, 25, 9, 6, 4, 3, 2, 1 };
    private int Top;
    private int PlayerBet;


    private void Start()
    {
        PlayAgainButton.SetActive(false);
        //Set Values
        Shuffle();
        DealCards();
        PlayerBet = 0;
        Top = 0;
        Balance.text += Player.balance;
        CurrentBet.text += PlayerBet;
    }

    #region CardDealing
    private void Deal(CardObject c)
    {
        //Deal card from top of the deck
        Card newCard = RemoveFromTop();
        c.myCard = newCard;
        //Debug.Log("Card Dealt: " + newCard.name);
        c.UpdateImage();
    }

    public Card RemoveFromTop()
    {
        //Remove card from top of deck
        Card TopCard = Deck[Top];
        RemoveCard(Top);

        if (Top >= 51)
            Top = 0;
        else
        {
            Top++;
        }
        return TopCard;
    }

    public void DealCards()
    {
        //Debug.Log("Grabbing Cards");
        List<GameObject> Hand = Player.ShowHand();
        Deal(Hand);
    }

    //Deal new Hand for Player
    private void Deal(List<GameObject> cards)
    {
        List<Card> newCards = new List<Card>();
        //loop to assign cards
        for (int i = 0; i < cards.Count; i++)
        {
            CardObject c = cards[i].GetComponent<CardObject>();
            if (!c.Hold)
            {

                //Add discarded cards to deck
                AddtoDeck(c.myCard);
                Deal(c);
            }

        }

    }

    //Visual Effect
    IEnumerator HideCards()
    {
        //play Sound
        Source.clip = CardFlip;
        Source.Play();
        foreach (GameObject g in Player.ShowHand())
        {
            CardObject c = g.GetComponent<CardObject>();
            if(!c.Hold)
                c.SetImage(CardDown);
        }

        //Debug.Log("Card Image called");
        yield return new WaitForSeconds(2);
        foreach (GameObject g in Player.ShowHand())
        {
            CardObject c = g.GetComponent<CardObject>();
            c.SetImage(c.myCard.CardFace);
        }
        //play Sound
        Source.clip = CardFlip;
        Source.Play();
        //Check Player's Hand
        ReadCards(Player.ShowCards());

    }

    void AddtoDeck(Card c)
    {
        Deck.Add(c);
    }

    //Shuffle Deck
    void Shuffle()
    {
        for (var i = 0; i < Deck.Count; i++)
        {
            int newPlace = UnityEngine.Random.Range(i, Deck.Count);
            Card temp = Deck[i];
            Deck[i] = Deck[newPlace];
            Deck[newPlace] = temp;
        }
    }

    //Remove Card from deck
    void RemoveCard(int index)
    {
        Deck.RemoveAt(index);
    }
    #endregion

    #region UI
    public void Bet1()
    {
        if (PlayerBet != 5)
        {
            Player.balance--;
            PlayerBet++;
            CurrentBet.text = "Bet: " + PlayerBet;
            UpdateBalance();
        }

    }

    public void BetMax()
    {
        if(PlayerBet != 5)
        {
            Player.balance -= 5;
            CurrentBet.text = "Bet: 5";
            PlayerBet = 5;
            UpdateBalance();
        }

    }

    //Update Player's Balance
    public void UpdateBalance(int income)
    {
        Player.balance += income;
        Balance.text = "Balance: " + Player.balance.ToString();
    }

    //Update Player's Balance text
    public void UpdateBalance()
    {
        Balance.text = "Balance: " + Player.balance.ToString();
    }

    public void Deal()
    {
        DealCards();
        StartCoroutine(HideCards());
        DealButton.SetActive(false);
    }


    public void PlayAgain()
    {
        Player.ResetHolds();
        Shuffle();
        Deal(Player.ShowHand());
        PlayerBet = 0;
        Balance.text = "Balance: " + Player.GetBalance().ToString();
        CurrentBet.text = "Bet: " + PlayerBet;
        WinType.text = "";
        PlayAgainButton.SetActive(false);
        DealButton.SetActive(true);
    }
    #endregion

    #region Win Logic
    //Read Player Card
    private void ReadCards(List<Card> cards)
    {
        int[] values = new int[5];
        values = Player.GetValues(); //Get Card Values
        Array.Sort(values); //Sort values in ascending order\


        //Bool Values to Determine Win Type
        bool isFlush = SameSuit(cards);
        bool isStraight = Straight(values);
        bool isRoyal = Royal(values);
        bool is2 = false;
        bool is3 = false;
        bool is4 = false;
        bool isJackorBetter = JackOrBetter(cards);

        //Make Values into list
        List<int> valList = values.ToList();
        List<int> pairs = new List<int>();
        int Count = 0;

        //Loop through hand
        for (int i = 0; i < valList.Count; i++)
        {
            //Count the number of same value cards we have
            Count = valList.Where(x => x.Equals(valList[i])).Count();
            switch (Count)
            {
                case (1):
                    break;
                case (2):
                    //Debug.Log("Two Pair");
                    is2 = true;
                    pairs.Add(valList[i]);
                    break;
                case (3):
                    //Debug.Log("Three of a kind");
                    is3 = true;
                    break;
                case (4):
                    //  Debug.Log("Four of a Kind");
                    is4 = true;
                    break;
            }
        }
        //Check for multiple Pairs
        bool isTwoPair = TwoPairs(pairs);

        //Check what the Player Wins
        CheckWin(isFlush, isRoyal, isStraight, is4, is3, is2, isTwoPair, isJackorBetter);

    }

    private void CheckWin(bool isFlush, bool isRoyal, bool isStraight, bool is4, bool is3, bool is2, bool isTwoPair, bool isJackorBetter)
    {
        //Check WinType

        //Royal Flush
        if (isFlush && isRoyal)
        {
            Payout(0, PlayerBet);
            //Debug.Log("Royal Flush");
        }
        else if (isStraight && isFlush) //Straight Flush
        {
            Payout(1, PlayerBet);
            //Debug.Log("Straight Flush");
        }
        else if (is4)//4 of a kind
        {
            Payout(2, PlayerBet);
            // Debug.Log("Four of a Kind");
        }
        else if (is2 && is3) //Full House
        {
            Payout(3, PlayerBet);
            //Debug.Log("Full House");
        }
        else if (isFlush)  //Flush
        {
            Payout(4, PlayerBet);
        }
        else if (isStraight) //Straight
        {
            Payout(5, PlayerBet);
        }
        else if (is3)  //3 of a kind
        {
            Payout(6, PlayerBet);
        }
        else if (isTwoPair)  //Two Pair
        {
            Payout(7, PlayerBet);
        }
        else if (isJackorBetter)  //Jacks or better
        {
            Payout(8, PlayerBet);
            //Debug.Log("Better than nothing :/");
        }
        else
        {
            // Debug.Log("You lost :(");
            WinType.text = "You lost :(";
            PlayAgainButton.SetActive(true);
        }
    }

    //Check SameSuit (Flush)
    private bool SameSuit(List<Card> cards)
    {
        //Get 1st card
        Card card1 = cards[0];
        bool suit = false;
        foreach (Card c in cards)
        {
            if (card1.Suit.Equals(c.Suit))
            {
                suit = true;
                continue;

            }
            else
            {
                suit = false;
                break;
            }
        }
        return suit;
    }

    //Check if is straight
    private bool Straight(int[] values)
    {

        string CardVals = ValueToString(values);
        if (straights.Contains(CardVals))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Check if Royal
    private bool Royal(int[] values)
    {
        string CardVals = ValueToString(values);
        if (RoyalStraight.Equals(CardVals))
        {
            return true;
        }
        else { return false; }
    }
    //Convert ints to string
    private string ValueToString(int[] values)
    {
        string CardVals = "";
        foreach (int i in values)
        {
            CardVals += i.ToString();
        }

        return CardVals;
    }
    //Check Jacks or Better
    private bool JackOrBetter(List<Card> cards)
    {
        bool isJackorBetter = false;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].Value > 10 && !cards[i].isAce)
            {
                isJackorBetter = true;
            }

        }
        return isJackorBetter;
    }
    //Check if TwoPairs
    private bool TwoPairs(List<int> pairs)
    {
        bool isPairs = false;
        for (int i = 0; i < pairs.Count; i++)
        {
            for(int j = i+1; j < pairs.Count; j++)
            {
                if(pairs[i] != pairs[j])
                {
                    //Debug.Log("Two different Pairs found");
                    isPairs = true;
                }

            }
            
        }

        return isPairs;
    }
    //Pay the Player
    private void Payout(int winIndex, int PlayerBet)
    {
       // Debug.Log("Payout: " + PayoutDefault[winIndex] * PlayerBet);
        //Debug.Log("Player Bet: " + PlayerBet);
        //Debug.Log("PayoutDefault: " + winIndex);
        Player.balance += PayoutDefault[winIndex] * PlayerBet;
        UpdateBalance();
        WinType.text = "You won: " + Wins[winIndex].ToString();
        PlayAgainButton.SetActive(true);
    }
    #endregion
}
