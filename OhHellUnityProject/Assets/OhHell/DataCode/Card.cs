using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Card
{
    public CardSuit Suit;
    public int Power;

    public Card() { }
    public Card(CardSuit suit, int power)
    {
        Suit = suit;
        if (power < 1 || power > 13)
        {
            throw new System.Exception("invalid card power");
        }
        Power = power;
    }

    public override string ToString()
    {
        return Suit + " " + Power;
    }
}

public class CardEvent : UnityEvent<GameObject, Card>
{
}

public enum CardSuit
{
    Diamond = 0,
    Heart,
    Spade,
    Club
}
