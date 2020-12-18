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
    public Card() { }
    public Card(CardSuit suit, int power)
    {
        Suit = suit;
        if(power < 1 || power > 13)
        {
            throw new System.Exception("invalid card power");
        }
        Power = power;
    }

    public string ToString()
    {
        return "suit= " + Suit + "  power= "+Power;
    }
    public CardSuit Suit;
    public int Power;
}

public class CardVisualBehavior : MonoBehaviour
{
    public List<Sprite> cardSprites;
    private Card myCard;
    private SpriteRenderer spriteRenderer;
    
    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetCard(new Card(CardSuit.Heart, 3));
    }
    public void SetCard(Card card)
    {
        myCard = card;
        SetSprite(card.Suit, card.Power);
    }
    private void SetSprite(CardSuit suit, int cardPower)
    {
        spriteRenderer.sprite = GetSprite(suit, cardPower);
    }
    private Sprite GetSprite(CardSuit suit, int cardPower)
    {
        int index = (int) suit * 13 + cardPower - 1;
        return cardSprites[index];
    }

    void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.50f, 0.50f);
        //If your mouse hovers over the GameObject with the script attached, output this message
      //  Debug.Log("Mouse is over GameObject.");
    }

    void OnMouseExit()
    {
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f);
        //The mouse is no longer hovering over the GameObject so output this message each frame
       // Debug.Log("Mouse is no longer on GameObject.");
    }

    private void OnMouseUp()
    {

        Debug.Log(myCard.ToString() + "  clicked");
    }
}
