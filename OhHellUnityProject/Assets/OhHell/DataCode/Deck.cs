using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Deck
{
    private List<Card> Cards;
    public Deck()
    {
        Cards = new List<Card>();
        for (int i = 0; i < 4; i++)
        {
            AddSuit((CardSuit)i);
        }
        Shuffle();
    }
    private void AddSuit(CardSuit suit)
    {
        for (int i = 1; i < 14; i++)
        {
            Cards.Add(new Card(suit, i));
        }
    }
    private void Shuffle()
    {
        for (int i = 0; i < 1000; i++)
        {
            Card card = Cards[0];
            Cards.RemoveAt(0);
            int newPos = Random.Range(0, Cards.Count + 1);
            Cards.Insert(newPos, card);
        }
    }
    public Card DrawCard()
    {
        Card card = Cards[0];
        Cards.RemoveAt(0);
        return card;
    }

    public List<Card> DrawHand(int numCards)
    {
        List<Card> hand = new List<Card>();
        for (int i = 0; i < numCards; i++)
        {
            hand.Add(DrawCard());
        }
        return hand;
    }
}