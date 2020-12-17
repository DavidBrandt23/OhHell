using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        for(int i = 0; i < 1000; i++)
        {
            Card card = Cards[0];
            Cards.RemoveAt(0);
            int newPos = Random.Range(0, Cards.Count+1);
            Cards.Insert(newPos, card);
        }
    }
    public Card DrawCard()
    {
        Card card = Cards[0];
        Cards.RemoveAt(0);
        return card;
    }

}
public class CardHandBehavior : MonoBehaviour
{
    public List<Card> Cards;
    public GameObject CardPrefab;
    // Start is called before the first frame update
    void Start()
    {
        List<Card> newHand = GetRandomHand();
        SetCards(newHand);
    }

    public void SetCards(List<Card> newCards)
    {
        Cards = newCards;

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        float cardSpacing = 12.0f;
        float firstCardOffset = -1 * (Cards.Count - 1) / 2.0f * cardSpacing;
        for (int i = 0; i < Cards.Count; i++)
        {
            GameObject newCard = Instantiate(CardPrefab, transform);
            newCard.GetComponent<CardVisualBehavior>().SetCard(Cards[i]);

            newCard.transform.localPosition = new Vector3(firstCardOffset + i * cardSpacing, 0.0f, 0.0f);
        }
    }
    private List<Card> GetRandomHand()
    {
        List<Card> hand = new List<Card>();
        Deck deck = new Deck();
        for (int i = 0; i < 6; i++)
        {
            hand.Add(deck.DrawCard());
        }
        return hand;
    }

    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            List<Card> newHand = GetRandomHand();
            SetCards(newHand);
        }
    }

}
