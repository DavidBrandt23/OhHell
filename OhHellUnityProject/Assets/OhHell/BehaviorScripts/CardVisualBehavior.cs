using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CardVisualBehavior : MonoBehaviour
{
    public List<Sprite> cardSprites;
    private SpriteRenderer spriteRenderer;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    public void SetCard(Card card)
    {
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
}
