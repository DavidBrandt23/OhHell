using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerOhHell : NetworkBehaviour
{
    public GameObject playerViewSelfPrefab;
    public GameObject playerViewOtherPrefab;

    [SyncVar(hook = nameof(SetPlayerName))]
    public string PlayerName;

    [SyncVar]
    public uint gameManagerNetId;
    
    [SyncVar(hook = nameof(SetTricks))]
    public int TricksThisRound;
    
    [SyncVar(hook = nameof(SetCurrentRoundBid))]
    public int CurrentRoundBid;

    [SyncVar(hook = nameof(SetCurrentScore))]
    public int CurrentScore;

    [SyncVar(hook = nameof(SetIsMyTurn))]
    public bool IsMyTurn;

    private OtherPlayerViewBehavior otherPlayerViewBehavior;
    private PlayerSelfViewBehavior playerSelfViewBehavior;
    private void Update()
    {

        if (Input.GetKeyUp("space"))
        {
          //  SceneManager.LoadScene("MenuScene");
            // List<Card> newHand = GetRandomHand();
            // SetCards(newHand);
        }
    }
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
        DataNeededForPlayerUI data = SelfGatherDataNeededForPlayerUI();
        if (isLocalPlayer)
        {
            playerSelfViewBehavior?.RefreshUI(data);
        }
        else
        {
            otherPlayerViewBehavior?.RefreshUI(data);
        }
    }
    private DataNeededForPlayerUI SelfGatherDataNeededForPlayerUI()
    {
        GameManager gameManager = GetGameManager();
        int? bidToShow = CurrentRoundBid;
        bool showOtherBids = false;
        if (gameManager == null)
        {
            showOtherBids = false;
        }
        else
        {
            showOtherBids = gameManager.ShowOtherBids;
        }

        if(!isLocalPlayer && !showOtherBids)
        {
            bidToShow = null;
        }
        return new DataNeededForPlayerUI
        {
            leadingSuit = gameManager?.GetLeadingSuit(),
            trumpCard = gameManager?.TrumpCard,
            currentTricks = TricksThisRound,
            isMyTurn = IsMyTurn,
            currentBid = bidToShow,
            currentScore = CurrentScore
        };

    }
    void SetCurrentScore(int oldScore, int newScore)
    {
        CurrentScore = newScore;
        UpdateSelfPlayerUI();
    }
    void SetTricks(int oldScore, int newScore)
    {
        TricksThisRound = newScore;
        UpdateSelfPlayerUI();
    }
    void SetCurrentRoundBid(int oldScore, int newScore)
    {
        CurrentRoundBid = newScore;
        UpdateSelfPlayerUI();
    }
    //On client
    [ClientRpc]
    public void RoundStart(List<Card> hand)
    {
        if (isLocalPlayer)
        {
            playerSelfViewBehavior.OnNewRound(hand, GetGameManager().GetTrickLeaderName());
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
            playerSelfViewBehavior.CardSelectedEvent.AddListener(OnCardChosen);
            playerSelfViewBehavior.BidEvent.AddListener(OnBidChosen);


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
            UpdateSelfPlayerUI();
            // Text text = GameObject.Find("PVOtext").GetComponent<Text>();
            // text.text = "Other player is" + amount;
            //    (NetworkManager. as NetworkManagerOhHell).CommandOne();
        }

    }

    [ClientRpc]
    public void SendSelfUIUpdate()
    {
        UpdateSelfPlayerUI();
    }

    private void OnCardChosen(GameObject sourceObj, Card card)
    {
        CmdCardChosen(card);
    }
    private void OnBidChosen(int bid)
    {
       // Debug.Log("player bid = " + bid);
        CmdBidChosen(bid);
    }
    [ClientRpc]
    public void GetInputPlayerName()
    {
        if (isLocalPlayer)
        {
            GameObject input = GameObject.Find("NetworkManager");
            string name = input.GetComponent<NetworkManagerOhHell>().localPlayerName;
            CmdUpdatePlayerInputName(name);
        }
    }
    [Command]
    public void CmdUpdatePlayerInputName(string name)
    {
        PlayerName = name;
        GameObject.Find("NetworkManager").GetComponent<NetworkManagerOhHell>().UpdateLobbyNames();
    }

    //SERVER
    [Command]
    public void CmdCardChosen(Card card)
    {
        PlayCard(card); //send to other clients
        GetGameManager().CardChosen(this, card);
    }
    //SERVER
    [Command]
    public void CmdBidChosen(int bid)
    {
        CurrentRoundBid = bid;
        //PlayCard(card); //send to other clients
        GetGameManager().BidChosen(this);
    }
    private GameManager GetGameManager()
    {
        if (NetworkIdentity.spawned.TryGetValue(gameManagerNetId, out NetworkIdentity identity))
        {
            return identity.gameObject.GetComponent<GameManager>();
        }
        Debug.Log("failed to get gamemanager using ID + " + gameManagerNetId);
        return null;
    }

}
