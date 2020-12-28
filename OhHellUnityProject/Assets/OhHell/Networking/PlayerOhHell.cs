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

    [SyncVar]
    public int ScoreLastRound;

    [SyncVar(hook = nameof(SetIsMyTurn))]
    public bool IsMyTurn;

    private OtherPlayerViewBehavior otherPlayerViewBehavior;
    private PlayerSelfViewBehavior playerSelfViewBehavior;

    private void Update()
    {
        if (Input.GetKeyUp("space"))
        {
        }
    }

    void SetPlayerName(string oldVal, string newVal)
    {
        PlayerName = newVal;
        UpdateSelfPlayerUI();
    }

    void SetIsMyTurn(bool oldVal, bool newVal)
    {
        IsMyTurn = newVal;
        UpdateSelfPlayerUI();
    }

    void SetCurrentScore(int oldVal, int newVal)
    {
        CurrentScore = newVal;
        UpdateSelfPlayerUI();
    }

    void SetTricks(int oldVal, int newVal)
    {
        TricksThisRound = newVal;
        UpdateSelfPlayerUI();
    }

    void SetCurrentRoundBid(int oldVal, int newVal)
    {
        GameManager gameManager = GetGameManager();
        Vector3 handPos = gameManager.GetPlayerHandPosition(this);
        Vector3 facingPos = gameManager.getBidFacingPos();
        if (oldVal == -1)
        {
            if (isLocalPlayer)
            {
                playerSelfViewBehavior.CreateBidHand(handPos, facingPos);
            }
            else
            {
                otherPlayerViewBehavior.CreateBidHand(handPos, facingPos);
            }
        }
        CurrentRoundBid = newVal;
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

        bool isTrickWinner = false;
        if(gameManager != null)
        {
            isTrickWinner = gameManager.PlayerIsTrickWinner(this);
        }
        else
        {

        }
        return new DataNeededForPlayerUI
        {
            leadingSuit = gameManager?.GetLeadingSuit(),
            trumpCard = gameManager?.TrumpCard,
            currentTricks = TricksThisRound,
            isMyTurn = IsMyTurn,
            currentBid = bidToShow,
            currentScore = CurrentScore,
            playerName = PlayerName,
            isTrickWinner = isTrickWinner,
            trickWinnerNameToShow = gameManager?.GetTrickWinnerName(),
        };

    }

    [ClientRpc]
    public void RpcRoundStart(List<Card> hand, bool isIndianRound)
    {
        if (isLocalPlayer)
        {
            playerSelfViewBehavior?.OnNewRound(hand, GetGameManager().GetTrickLeaderName(), isIndianRound);
            //right now when dealing hand the new cards are always non-clickable by default so call update
            UpdateSelfPlayerUI();
        }
        else
        {
            otherPlayerViewBehavior?.OnNewRound(hand, isIndianRound);
        }
    }

    //server calls this on player objects when a remote player plays a card
    [ClientRpc]
    public void RpcPlayCard(Card card)
    {
        if (!isLocalPlayer)
        {
            otherPlayerViewBehavior.PlayCard(card);
        }
    }

    [ClientRpc]
    public void RpcTrickEnd()
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
    public void RpcStartPostRound()
    {
        GameManager gameManager = GetGameManager();
        Vector3 handPos = gameManager.GetPlayerHandPosition(this);
        if (isLocalPlayer)
        {
            playerSelfViewBehavior.PostRound(ScoreLastRound, handPos);
        }
        else
        {
            otherPlayerViewBehavior.PostRound(ScoreLastRound, handPos);
        }
    }

    [ClientRpc]
    public void RpcInitiateBidPhase()
    {
        GameManager gameManager = GetGameManager();
        Vector3 handPos = gameManager.GetPlayerHandPosition(this);
        Vector3 facingPos = gameManager.getBidFacingPos();
        bool fire = gameManager.DidAnyBidFire();
        if (isLocalPlayer)
        {
            playerSelfViewBehavior.StartBidPhase(handPos, facingPos, CurrentRoundBid, fire);
        }
        else
        {
            otherPlayerViewBehavior.StartBidPhase(handPos, facingPos, CurrentRoundBid, fire);
        }
    }

    [ClientRpc]
    public void RpcEndBidPhase()
    {
        if (isLocalPlayer)
        {
            playerSelfViewBehavior.EndBidPhase();
        }
        else
        {
            otherPlayerViewBehavior.EndBidPhase();
        }
    }

    [ClientRpc]
    public void RpcInitiateScoreboard(bool isHalfTime)
    {
        if (isLocalPlayer)
        {
            playerSelfViewBehavior.ShowScores(GetGameManager().GetScores(), isHalfTime);
        }
    }

    [ClientRpc]
    public void RpcInitializeUI(uint newGameManagerNetId)
    {
        gameManagerNetId = newGameManagerNetId;
        if (isLocalPlayer)
        {
            GameObject myPlayerUI = Instantiate(playerViewSelfPrefab); ;
            playerSelfViewBehavior = myPlayerUI.GetComponent<PlayerSelfViewBehavior>();
            playerSelfViewBehavior.Initialize();
            UpdateSelfPlayerUI();
            playerSelfViewBehavior.CardSelectedEvent.AddListener(OnCardChosen);
            playerSelfViewBehavior.BidEvent.AddListener(OnBidChosen);
        }
        else
        {
            GameObject ob = Instantiate(playerViewOtherPrefab);
            otherPlayerViewBehavior = ob.GetComponent<OtherPlayerViewBehavior>();
            ob.transform.position = GetGameManager().GetPlayerPosition(this);
            otherPlayerViewBehavior.CardTargetPoint.transform.position = GetGameManager().GetPlayerCardTargetPosition(this);
            UpdateSelfPlayerUI();
        }
    }

    [ClientRpc]
    public void RpcSendSelfUIUpdate()
    {
        UpdateSelfPlayerUI();
    }

    [ClientRpc]
    public void RpcGetInputPlayerName()
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
    
    [Command]
    public void CmdCardChosen(Card card)
    {
        RpcPlayCard(card); //send to other clients
        GetGameManager().CardChosen(this, card);
    }

    [Command]
    public void CmdBidChosen(int bid)
    {
        CurrentRoundBid = bid;
        GetGameManager().BidChosen(this);
    }

    private GameManager GetGameManager()
    {
        if (NetworkIdentity.spawned.TryGetValue(gameManagerNetId, out NetworkIdentity identity))
        {
            return identity.gameObject.GetComponent<GameManager>();
        }

        return null;
    }

    private void OnCardChosen(GameObject sourceObj, Card card)
    {
        CmdCardChosen(card);
    }

    private void OnBidChosen(int bid)
    {
        CmdBidChosen(bid);
    }
}
