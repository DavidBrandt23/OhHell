using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelfViewBehavior : MonoBehaviour
{
    public CardHandBehavior cardHandBehavior;
    public Text TurnText;
    public GameObject CardTarget;

    public CardEvent CardSelectedEvent;
    private GameObject ThrownCard;
    public void Awake()
    {
        CardSelectedEvent = new CardEvent();
       // cardHandBehavior.CanChooseCard = true;//tests
        //cardHandBehavior.ClickCardEvent.AddListener(onCardSelected);//for test
    }
    public void Initialize()
    {
        cardHandBehavior.CanChooseCard = false;
        cardHandBehavior.ClickCardEvent.AddListener(onCardSelected);
    }
    private void onCardSelected(GameObject sourceCardGameObject, Card card)
    {
        ThrownCard = sourceCardGameObject;
        //ThrownCard.transform.SetParent(gameObject.transform); //move out of card hand
        OtherPlayerViewBehavior.ThrowCard(ThrownCard, CardTarget.transform.position);

        CardSelectedEvent.Invoke(sourceCardGameObject, card);
    }

    public void TrickEnd()
    {
        if (ThrownCard != null)
        {
            Destroy(ThrownCard);
            ThrownCard = null;
        }
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
