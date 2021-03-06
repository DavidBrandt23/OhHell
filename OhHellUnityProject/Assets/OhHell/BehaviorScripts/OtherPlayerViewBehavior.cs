﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSharedViewBehavior : MonoBehaviour
{
    public GameObject BiddingHandPrefab;
    protected GameObject ActiveBidObj;
    public void CreateBidHand(Vector3 handPos, Vector3 posToFace)
    {
        if(ActiveBidObj == null)
        {
            ActiveBidObj = Instantiate(BiddingHandPrefab);
            BiddingHandBehavior bhb = ActiveBidObj.GetComponent<BiddingHandBehavior>();
            ActiveBidObj.transform.position = handPos;

            bhb.HandObject.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), GetAngle(new Vector3(handPos.x, handPos.y + 1.0f, handPos.z) - handPos, posToFace - handPos));
        }
    }

    //Gets clockwise angle
    private static float GetAngle(Vector2 v1, Vector2 v2)
    {
        var sign = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
        return Vector2.Angle(v1, v2) * sign;
    }

    public virtual void StartBidPhase(Vector3 handPos, Vector3 posToFace, int bid, bool fire)
    {
        CreateBidHand(handPos, posToFace);
        ActiveBidObj.GetComponent<BiddingHandBehavior>().StartBid(bid);
    }

    public void EndBidPhase()
    {
        if(ActiveBidObj != null)
        {
            Destroy(ActiveBidObj);
            ActiveBidObj = null;
        }
    }
}

public class OtherPlayerViewBehavior : PlayerSharedViewBehavior
{
    public GameObject CardSpawnPoint;
    public GameObject CardTargetPoint;
    public CardThrowBehavior cardThrowBehavior;
    public RoundScoreMakerBehavior roundScoreMakerBehavior;
    public PlayerInfoBoxBehavior playerInfoBox;

    private GameObject IndianCard;
    private GameObject ThrownCard;
    private GameObject ActiveRoundScore;

    public void RefreshUI(DataNeededForPlayerUI data)
    {
        playerInfoBox.UpdateUI(data);

        if (data.isTrickWinner)
        {
            if (ThrownCard != null)
            {
                ThrownCard.GetComponent<CardVisualBehavior>().EnableHighlight(true);
            }
        }
    }

    public void OnNewRound(List<Card> hand, bool isIndianRound)
    {
        if (isIndianRound)
        {
            Card card = hand[0];
            Vector3 cardSpawn = CardSpawnPoint.transform.position;
            Vector3 higher = new Vector3(cardSpawn.x, cardSpawn.y, cardSpawn.z - 0.6f);
            IndianCard = cardThrowBehavior.ThrowCard(card, higher, higher, true);

        }
        if (ActiveRoundScore != null)
        {
            Destroy(ActiveRoundScore);
            ActiveRoundScore = null;
        }
    }
    
    public void PlayCard(Card card)
    {
        if(IndianCard != null)
        {
            Destroy(IndianCard);
            IndianCard = null;
        }
        ThrownCard = cardThrowBehavior.ThrowCard(card, CardSpawnPoint.transform.position, CardTargetPoint.transform.position);
    }

    public void TrickEnd()
    {
        if(ThrownCard != null)
        {
            Destroy(ThrownCard);
            ThrownCard = null;
        }
    }
    
    public void PostRound(int scoreLastRound, Vector3 position)
    {
        ActiveRoundScore = roundScoreMakerBehavior.MakeScoreObject(scoreLastRound, position);
    }
}
