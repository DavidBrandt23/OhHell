using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardSuit
{
    Diamond = 0,
    Heart,
    Spade,
    Club
}

public class Card
{
    public Card(CardSuit suit, int power)
    {
        Suit = suit;
        if(power < 1 || power > 13)
        {
            throw new System.Exception("invalid card power");
        }
        Power = power;
    }
    public CardSuit Suit;
    public int Power;
}

public class CardVisualBehavior : MonoBehaviour
{
    public List<Sprite> cardSprites;
    private Card myCard;
    public void SetCard(Card card)
    {
        myCard = card;
        SetSprite(card.Suit, card.Power);
    }
    private void SetSprite(CardSuit suit, int cardPower)
    {
        GetComponent<SpriteRenderer>().sprite = GetSprite(suit, cardPower);
    }
    private Sprite GetSprite(CardSuit suit, int cardPower)
    {
        int index = (int) suit * 13 + cardPower - 1;
        return cardSprites[index];
    }
}
