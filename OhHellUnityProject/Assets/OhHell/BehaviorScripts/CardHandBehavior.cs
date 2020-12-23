using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CardHandBehavior : MonoBehaviour
{
    public List<Card> Cards;
    private List<GameObject> cardObjects;
    public GameObject CardPrefab;
    public CardEvent ClickCardEvent;
    public bool TestMode;


    public void SetSelectableCards(bool canSelect, CardSuit? allowedSuit = null)
    {
        foreach (GameObject cardObj in cardObjects)
        {
            if (cardObj) //in case destroyed and didn't cleanup list
            {
                CardBehavior cardB = cardObj.GetComponent<CardBehavior>();
                SelectableObjectBehavior selBehave = cardObj.GetComponent<SelectableObjectBehavior>();
                if (canSelect && (allowedSuit == null || !hasCardOfSuit((CardSuit)allowedSuit) || (cardB.GetCard().Suit == allowedSuit)))
                {
                    selBehave.ClickEnabled = true;
                }
                else
                {
                    selBehave.ClickEnabled = false;
                }
            }
        }
    }
    private bool hasCardOfSuit(CardSuit suit)
    {

        foreach (GameObject cardObj in cardObjects)
        {
            if (cardObj) //in case destroyed and didn't cleanup list
            {
                CardBehavior cardB = cardObj.GetComponent<CardBehavior>();
                if(cardB.GetCard().Suit == suit)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Awake()
    {
        cardObjects = new List<GameObject>();
        ClickCardEvent = new CardEvent();
        if(TestMode)
        {
            Deck testDeck = new Deck();
            SetCards(testDeck.DrawHand(6));
            SetSelectableCards(true);
        }
    }

    public void SetCards(List<Card> newCards, bool faceDown = false)
    {
        Cards = newCards;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        cardObjects.Clear();

        float cardSpacing = 12.0f;
        float firstCardOffset = -1 * (Cards.Count - 1) / 2.0f * cardSpacing;
        for (int i = 0; i < Cards.Count; i++)
        {
            GameObject newCard = Instantiate(CardPrefab, transform);
            cardObjects.Add(newCard);

            CardBehavior cardBehavior = newCard.GetComponent<CardBehavior>();
            SelectableObjectBehavior selectableObjectBehavior = newCard.GetComponent<SelectableObjectBehavior>();
            //selectableObjectBehavior.ClickEnabled = CanChooseCard;
            cardBehavior.SetCard(Cards[i], faceDown);
            cardBehavior.CardSelectedEvent.AddListener(OnCardClick);

            newCard.transform.localPosition = new Vector3(firstCardOffset + i * cardSpacing, 0.0f, 0.0f);
        }
    }

    private void OnCardClick(GameObject sourceCardGameObject, Card card)
    {
        ClickCardEvent.Invoke(sourceCardGameObject, card);
        //old
        //sourceCardGameObject.GetComponent<SelectableObjectBehavior>().ClickEnabled = false;
        Destroy(sourceCardGameObject);
        cardObjects.Remove(sourceCardGameObject);

        //TODO; make this better
        //Destroy(sourceCardGameObject.GetComponent<SelectableObjectBehavior>());
       // sourceCardGameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);

        SetSelectableCards(false);
    }


    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
           // List<Card> newHand = GetRandomHand();
           // SetCards(newHand);
        }
    }

}
