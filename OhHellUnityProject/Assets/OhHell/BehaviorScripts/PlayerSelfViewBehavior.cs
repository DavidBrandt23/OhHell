using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DataNeededForPlayerUI
{
    public int? currentBid;
    public int? currentTricks;
    public CardSuit? leadingSuit;
    public bool isMyTurn;
    public Card trumpCard;
    public int? currentScore;
}

public class PlayerSelfViewBehavior : MonoBehaviour
{
    public CardHandBehavior cardHandBehavior;
    public Text TurnText;
    public Text TricksText;
    public Text ScoreText;
    public Text BidText;
    //  public Text TrumpLabelText;
    public GameObject CardTarget;
    public CardBehavior TrumpCardScript;
    public GameObject bidUIPrefab;
    public BidSelectedEvent BidEvent;
    public CardEvent CardSelectedEvent;
    private GameObject ThrownCard;
    public AudioSource myAudioSource;

    public AudioClip DealSound;

    private BidUIBehavior ActiveBidUI;
    public void Awake()
    {
        CardSelectedEvent = new CardEvent();

        BidEvent = new BidSelectedEvent();
       // cardHandBehavior.CanChooseCard = true;//tests
        //cardHandBehavior.ClickCardEvent.AddListener(onCardSelected);//for test
    }
    public void RefreshUI(DataNeededForPlayerUI data)
    {
        cardHandBehavior.SetSelectableCards(data.isMyTurn, data.leadingSuit);
        TurnText.enabled = data.isMyTurn;
        TrumpCardScript.SetCard(data.trumpCard);
        TricksText.text = OtherPlayerViewBehavior.TricksDisplayString(data.currentTricks);
        BidText.text = OtherPlayerViewBehavior.BidDisplayString(data.currentBid);
        ScoreText.text = OtherPlayerViewBehavior.ScoreDisplayString(data.currentScore);
    }
    public void Initialize()
    {
        cardHandBehavior.SetSelectableCards(false);
        cardHandBehavior.ClickCardEvent.AddListener(onCardSelected);
    }
    private void onCardSelected(GameObject sourceCardGameObject, Card card)
    {
        //sourceCardGameObject will be destroyed after this

        // ThrownCard = sourceCardGameObject;
        //ThrownCard.transform.SetParent(gameObject.transform); //move out of card hand
        ThrownCard = GetComponent<CardThrowBehavior>().ThrowCard(card, sourceCardGameObject.transform.position, CardTarget.transform.position); 

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
    public void OnNewRound(List<Card> newCards, string trickLeaderName, bool isIndianRound)
    {
        cardHandBehavior.SetCards(newCards, isIndianRound);
        GameObject bidUIObj = Instantiate(bidUIPrefab);
        ActiveBidUI = bidUIObj.GetComponent<BidUIBehavior>();
        ActiveBidUI.SetupBidUI(newCards.Count, trickLeaderName, isIndianRound);

        ActiveBidUI.BidEvent.AddListener(OnBidChosen);
        myAudioSource.PlayOneShot(DealSound);
    }
    private void OnBidChosen(int bidValue)
    {
        BidEvent.Invoke(bidValue);
        Destroy(ActiveBidUI.gameObject);
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
