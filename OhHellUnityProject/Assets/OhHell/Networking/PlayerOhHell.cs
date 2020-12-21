﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOhHell : NetworkBehaviour
{
    public GameObject playerViewSelfPrefab;
    public GameObject playerViewOtherPrefab;

    [SyncVar(hook = nameof(SetPlayerName))]
    public string PlayerName;

    [SyncVar]
    public uint gameManagerNetId;
    
    [SyncVar(hook = nameof(SetScore))]
    public int Score;

    [SyncVar(hook = nameof(SetIsMyTurn))]
    public bool IsMyTurn;

    private OtherPlayerViewBehavior otherPlayerViewBehavior;
    private PlayerSelfViewBehavior playerSelfViewBehavior;

    void FixedUpdate()
    {
    }
    void SetPlayerName(string oldColor, string newColor)
    {
        PlayerName = newColor;
        if (!isLocalPlayer)
        {
            otherPlayerViewBehavior?.UpdatePlayerName(PlayerName);
        }
    }
    void SetIsMyTurn(bool oldColor, bool newColor)
    {
        IsMyTurn = newColor;
        UpdateSelfPlayerUI();
    }

    private void UpdateSelfPlayerUI()
    {
        if (isLocalPlayer)
        {
            playerSelfViewBehavior?.UpdateTurnUI(IsMyTurn, GetGameManager()?.GetLeadingSuit());
        }
    }

    void SetScore(int oldScore, int newScore)
    {
        Score = newScore;
        if (isLocalPlayer)
        {
            playerSelfViewBehavior?.UpdateScoreUI(Score);
        }
        else
        {
            otherPlayerViewBehavior?.UpdateScoreUI(Score);
        }
    }
    //On client
    [ClientRpc]
    public void RoundStart(List<Card> hand)
    {
        if (isLocalPlayer)
        {
            playerSelfViewBehavior.OnNewRound(hand);
            //right now when dealing hand the new cards are always non-clickable by default so call update
            UpdateSelfPlayerUI();
        }
    }

    [ClientRpc]
    public void Display(string message)
    {
        if (isLocalPlayer)
        {
            //otherPlayerViewBehavior.messageDisplayer.SetMessage(message);

        }
    }

    [ClientRpc]
    public void YourTurn()
    {
        if (isLocalPlayer)
        {
           // playerSelfViewBehavior.OnMyTurn();
        }
    }

    [ClientRpc]
    public void PlayCard(Card card)
    {
        if (!isLocalPlayer)
        {
            otherPlayerViewBehavior.PlayCard(card);
        }
    }

    [ClientRpc]
    public void TrickEnd()
    {
        if (isLocalPlayer)
        {
            playerSelfViewBehavior.TrickEnd();
            UpdateSelfPlayerUI();
        }
        else
        {
            otherPlayerViewBehavior.TrickEnd();
        }
    }

    [ClientRpc]
    public void InitializeUI(uint amount)
    {
        gameManagerNetId =  amount;
        if (isLocalPlayer)
        {
            GameObject myPlayerUI = Instantiate(playerViewSelfPrefab); ;
            playerSelfViewBehavior = myPlayerUI.GetComponent<PlayerSelfViewBehavior>();
            playerSelfViewBehavior.Initialize();
            UpdateSelfPlayerUI();
            playerSelfViewBehavior.UpdateScoreUI(Score);
            playerSelfViewBehavior.CardSelectedEvent.AddListener(OnCardChosen);

            // GameObject otherPlayerUI = Instantiate(playerViewOtherPrefab);
            // otherPlayerViewBehavior = otherPlayerUI.GetComponent<OtherPlayerViewBehavior>();

        }
        else
        {
            GameObject ob = Instantiate(playerViewOtherPrefab);
            otherPlayerViewBehavior = ob.GetComponent<OtherPlayerViewBehavior>();
            otherPlayerViewBehavior.UpdatePlayerName(PlayerName);
            ob.transform.position = GetGameManager().GetPlayerPosition(this);
            otherPlayerViewBehavior.CardTargetPoint.transform.position = GetGameManager().GetPlayerCardTargetPosition(this);
            otherPlayerViewBehavior.UpdateScoreUI(Score);
            // Text text = GameObject.Find("PVOtext").GetComponent<Text>();
            // text.text = "Other player is" + amount;
            //    (NetworkManager. as NetworkManagerOhHell).CommandOne();
        }

    }
    private void OnCardChosen(GameObject sourceObj, Card card)
    {
        CmdCardChosen(card);
    }

    


    //SERVER
    [Command]
    public void CmdCardChosen(Card card)
    {
        PlayCard(card); //send to other clients
        GetGameManager().CardChosen(this, card);
    }
    private GameManager GetGameManager()
    {
        if (NetworkIdentity.spawned.TryGetValue(gameManagerNetId, out NetworkIdentity identity))
        {
            return identity.gameObject.GetComponent<GameManager>();
        }
        throw new Exception("failed to get gamemanager using ID + "+ gameManagerNetId);
    }

}
