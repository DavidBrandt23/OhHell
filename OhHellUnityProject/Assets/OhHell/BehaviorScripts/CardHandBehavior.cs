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

    private bool _canChooseCard;
    public bool CanChooseCard {
        get { return _canChooseCard; }
        set {
            _canChooseCard = value;
            foreach(GameObject ob in cardObjects)
            {
                if (ob)
                {
                    ob.GetComponent<SelectableObjectBehavior>().ClickEnabled = _canChooseCard;
                }
            }
        }
    }
    public void Awake()
    {
        cardObjects = new List<GameObject>();
        ClickCardEvent = new CardEvent();
        if(TestMode)
        {
            Deck testDeck = new Deck();
            SetCards(testDeck.DrawHand(6));
            CanChooseCard = true;
        }
    }

    public void SetCards(List<Card> newCards)
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
            selectableObjectBehavior.ClickEnabled = CanChooseCard;
            cardBehavior.SetCard(Cards[i]);
            cardBehavior.CardSelectedEvent.AddListener(OnCardClick);

            newCard.transform.localPosition = new Vector3(firstCardOffset + i * cardSpacing, 0.0f, 0.0f);
        }
    }

    private void OnCardClick(GameObject sourceCardGameObject, Card card)
    {
        ClickCardEvent.Invoke(sourceCardGameObject, card);
        sourceCardGameObject.GetComponent<SelectableObjectBehavior>().ClickEnabled = false;
        CanChooseCard = false;
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
