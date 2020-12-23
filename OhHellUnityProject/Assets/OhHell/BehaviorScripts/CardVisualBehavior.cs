using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CardVisualBehavior : MonoBehaviour
{
    public List<Sprite> cardSprites;
    public Sprite faceDownCardSprite;
    private SpriteRenderer spriteRenderer;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    public void SetCard(Card card, bool faceDown = false)
    {
        if (card == null)
        {
            spriteRenderer.sprite = null;
            return;
        }
        SetSprite(card.Suit, card.Power, faceDown);
    }

    private void SetSprite(CardSuit suit, int cardPower, bool faceDown)
    {
        spriteRenderer.sprite = GetSprite(suit, cardPower, faceDown);
    }

    private Sprite GetSprite(CardSuit suit, int cardPower, bool faceDown)
    {
        if (faceDown)
        {
            return faceDownCardSprite;
        }
        int index = (int) suit * 13 + cardPower - 1;
        return cardSprites[index];
    }
}
