using UnityEngine;
using System.Collections;

public class CardBehavior : MonoBehaviour
{
    private Card myCard;
    public CardEvent CardSelectedEvent ;
    public bool DebugMode;

    private void Awake()
    {
        CardSelectedEvent = new CardEvent();

        if (DebugMode)
        {
            SetCard(new Card(CardSuit.Spade, 13));
        }
    }

    public void onCardSelected()
    {
        Debug.Log("on card sel");
        CardSelectedEvent.Invoke(gameObject, myCard);
    }

    public void SetCard(Card card, bool faceDown = false)
    {
        myCard = card;
        GetComponent<CardVisualBehavior>().SetCard(myCard, faceDown);
    }

    public Card GetCard()
    {
        return myCard;
    }
}
