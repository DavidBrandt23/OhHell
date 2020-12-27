using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DataNeededForPlayerUI
{
    public string playerName;
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
    public TextMeshPro TurnText;
    public CardThrowBehavior cardThrowBehavior;

    public GameObject CardTarget;
    public CardBehavior TrumpCardScript;
    public GameObject bidUIPrefab;
    public BidSelectedEvent BidEvent;
    public CardEvent CardSelectedEvent;

    private GameObject ThrownCard;
    public AudioSource myAudioSource;
    public PlayerInfoBoxBehavior playerInfoBox;

    public AudioClip DealSound;
    public GameObject ScoreBoardPrefab;

    private BidUIBehavior ActiveBidUI;
    private ScoreboardBehavior ActiveScoreBoardUI;
    public void Awake()
    {
        CardSelectedEvent = new CardEvent();

        BidEvent = new BidSelectedEvent();
    }
    public void RefreshUI(DataNeededForPlayerUI data)
    {
        cardHandBehavior.SetSelectableCards(data.isMyTurn, data.leadingSuit);
        TurnText.enabled = data.isMyTurn;
        TrumpCardScript.SetCard(data.trumpCard);
        playerInfoBox.UpdateUI(data);
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
        ThrownCard = cardThrowBehavior.ThrowCard(card, sourceCardGameObject.transform.position, CardTarget.transform.position); 

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
        if(ActiveScoreBoardUI != null)
        {
            Destroy(ActiveScoreBoardUI.gameObject);
            ActiveScoreBoardUI = null;
        }
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

    public void ShowScores(List<ScorePair> list, bool isHalf)
    {
        GameObject newOb = Instantiate(ScoreBoardPrefab);
        ActiveScoreBoardUI = newOb.GetComponent<ScoreboardBehavior>();
        ActiveScoreBoardUI.UpdateScores(list, isHalf);
    }
}
