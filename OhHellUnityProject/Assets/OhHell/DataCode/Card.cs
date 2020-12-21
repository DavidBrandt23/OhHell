using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Mirror;

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

    public bool IsStronger(Card otherCard, CardSuit? leadSuit, CardSuit? trumpSuit)
    {
        if(otherCard == null)
        {
            return true;
        }

        if (Suit == trumpSuit && otherCard.Suit != trumpSuit)
        {
            return true;
        }

        if (otherCard.Suit == trumpSuit && Suit != trumpSuit)
        {
            return false;
        }

        if (Suit == leadSuit && otherCard.Suit != leadSuit)
        {
            return true;
        }

        if (otherCard.Suit == leadSuit && Suit != leadSuit)
        {
            return false;
        }

        return Power > otherCard.Power;
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
public static class CardSuixxtReaderWriterg
{
    public static void WriteCardSuit(this NetworkWriter writer, CardSuit dateTime)
    {
        writer.WriteInt64((long)dateTime);
    }

    public static CardSuit ReadCardSuit(this NetworkReader reader)
    {
        return (CardSuit)(reader.ReadInt64());
    }
}