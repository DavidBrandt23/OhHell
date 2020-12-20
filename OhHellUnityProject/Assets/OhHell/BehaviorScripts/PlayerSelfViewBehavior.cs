using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelfViewBehavior : MonoBehaviour
{
    public CardHandBehavior cardHandBehavior;
    public Text TurnText;

    public CardEvent CardSelectedEvent;

    public void Awake()
    {
        CardSelectedEvent = new CardEvent();
    }
    public void Initialize()
    {
        cardHandBehavior.CanChooseCard = false;
        cardHandBehavior.ClickCardEvent.AddListener(onCardSelected);
    }
    private void onCardSelected(GameObject source, Card card)
    {
        CardSelectedEvent.Invoke(source, card);
    }

    public void OnNewRound(List<Card> newCards)
    {
        cardHandBehavior.SetCards(newCards);
    }

    public void UpdateTurnUI(bool isMyTurn)
    {
        cardHandBehavior.CanChooseCard = isMyTurn;
        TurnText.enabled = isMyTurn;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
