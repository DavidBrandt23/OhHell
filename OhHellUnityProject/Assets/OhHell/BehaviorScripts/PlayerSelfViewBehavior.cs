using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
    public bool isTrickWinner;
    public string trickWinnerNameToShow;
}

public class PlayerSelfViewBehavior : PlayerSharedViewBehavior
{
    public CardHandBehavior cardHandBehavior;
    public TextMeshPro TurnText;
    public TextMeshPro TrickWinnerText;
    public CardThrowBehavior cardThrowBehavior;
    public RoundScoreMakerBehavior roundScoreMakerBehavior;

    public GameObject CardTarget;
    public CardBehavior TrumpCardScript;
    public GameObject bidUIPrefab;
    public BidSelectedEvent BidEvent;
    public CardEvent CardSelectedEvent;

    public AudioSource myAudioSource;
    public PlayerInfoBoxBehavior playerInfoBox;

    public AudioClip DealSound;
    public AudioClip BidThumpSound;
    public AudioClip BidFireSound;
    public AudioClip BidCorrectSound;
    public AudioClip BidWrongSound;
    public AudioClip YourTurnSound;
    public GameObject ScoreBoardPrefab;

    private GameObject ThrownCard;
    private BidUIBehavior ActiveBidUI;
    private ScoreboardBehavior ActiveScoreBoardUI;
    private GameObject ActiveRoundScore;

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

        string trickWinnerName = data.trickWinnerNameToShow;
        TrickWinnerText.enabled = trickWinnerName != null;
        if(trickWinnerName != null)
        {
            TrickWinnerText.text = trickWinnerName + " won the trick.";
        }

        if (data.isTrickWinner)
        {
            if(ThrownCard != null)
            {
                ThrownCard.GetComponent<CardVisualBehavior>().EnableHighlight(true);
            }
        }
    }

    public void Initialize()
    {
        cardHandBehavior.SetSelectableCards(false);
        cardHandBehavior.ClickCardEvent.AddListener(onCardSelected);
    }

    private void onCardSelected(GameObject sourceCardGameObject, Card card)
    {
        //sourceCardGameObject will be destroyed after this
        
        ThrownCard = cardThrowBehavior.ThrowCard(card, sourceCardGameObject.transform.position, CardTarget.transform.position); 

        CardSelectedEvent.Invoke(sourceCardGameObject, card);
    }

    public override void StartBidPhase(Vector3 handPos, Vector3 posToFace, int bid, bool fire)
    {
        base.StartBidPhase(handPos, posToFace, bid, fire);
        myAudioSource.PlayOneShot(BidThumpSound);
        if (fire)
        {
            ActiveBidObj.GetComponent<BiddingHandBehavior>().animDoneEvent.AddListener(OnBidAnimationDone);
        }
    }

    private void OnBidAnimationDone()
    {
        myAudioSource.PlayOneShot(BidFireSound);
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
        if(ActiveRoundScore != null)
        {
            Destroy(ActiveRoundScore);
            ActiveRoundScore = null;
        }
    }
    private void OnBidChosen(int bidValue)
    {
        BidEvent.Invoke(bidValue);
        Destroy(ActiveBidUI.gameObject);
    }

    public void ShowScores(List<ScorePair> list, bool isHalf)
    {
        GameObject newOb = Instantiate(ScoreBoardPrefab);
        ActiveScoreBoardUI = newOb.GetComponent<ScoreboardBehavior>();
        ActiveScoreBoardUI.UpdateScores(list, isHalf);
    }

    public void PostRound(int scoreLastRound, Vector3 position)
    {
        ActiveRoundScore = roundScoreMakerBehavior.MakeScoreObject(scoreLastRound, position);
        AudioClip soundToPlay = (scoreLastRound >= 10) ? BidCorrectSound : BidWrongSound;
        myAudioSource.PlayOneShot(soundToPlay, 0.6f);
    }

    public void PlayYourTurnSound()
    {
        myAudioSource.PlayOneShot(YourTurnSound, 0.4f);
    }
}
