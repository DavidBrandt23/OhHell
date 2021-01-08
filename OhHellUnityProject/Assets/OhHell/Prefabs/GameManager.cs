using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncListUInt : SyncList<uint> { }
public class GameManager : NetworkBehaviour
{
    public List<PlayerOhHell> players; //only works on server
    public SyncListUInt playerIds = new SyncListUInt();
    public SpawnPointBehavior spawnPointBehavior;

    [SyncVar]
    public Card TrumpCard;

    [SyncVar]
    private int currentTurnPlayerIndex = 0;

    [SyncVar]
    private int roundFirstLeader = 0;
    
    [SyncVar]
    private int LeadingSuit;

    [SyncVar]
    public bool ShowOtherBids;

    [SyncVar]
    public bool IsIndianRound;

    [SyncVar]
    private uint trickWinningPlayerId;
    
    private int CurrentRoundCardNum; 
    private bool RoundCardNumDecreasing;
    private int TricksPlayedThisRound;
    private List<Card> cardsInCenter;
    private int MaxHand = 6; //6 for final

    private PlayerOhHell TrickWinningPlayer; //keep winner id in sync
    private Card TrickWinningCard;

    public void Awake()
    {
        cardsInCenter = new List<Card>();
        SetLeadingSuit(null);
        CurrentRoundCardNum = 0; //0 for final version
        TricksPlayedThisRound = 0;
    }

    public List<string> GetPlayerNameList()
    {
        List<string> nameList = new List<string>();
        foreach(PlayerOhHell pl in GetLocalPlayerList())
        {
            nameList.Add(pl.PlayerName);
        }
        return nameList;
    }
    
    private PlayerOhHell GetLocalPlayer()
    {
        List<PlayerOhHell> localList = GetLocalPlayerList();
        for (int i = 0; i < localList.Count; i++)
        {
            if (localList[i].isLocalPlayer)
            {
                return localList[i];
            }
        }
        return null;
    }

    private int GetPlayerIndex(PlayerOhHell player)
    {
        List<PlayerOhHell> localList = GetLocalPlayerList();
        for (int i = 0; i < localList.Count; i++)
        {
            if (localList[i] == player)
            {
                return i;
            }
        }
        return -1;
    }

    public Vector3 getBidFacingPos()
    {
        return spawnPointBehavior.BidCenter.position;
    }

    private int NumPlayers()
    {
        return playerIds.Count;
    }

    public Vector3 GetPlayerPosition(PlayerOhHell player)
    {
        return spawnPointBehavior.GetSpawnPoint(NumPlayers(), GetPlayerPositionIndex(player));
    }

    public Vector3 GetPlayerCardTargetPosition(PlayerOhHell player)
    {
        return spawnPointBehavior.GetCardTarget(NumPlayers(), GetPlayerPositionIndex(player));
    }

    public Vector3 GetPlayerHandPosition(PlayerOhHell player)
    {
        int index = GetPlayerPositionIndex(player);
        if (player.isLocalPlayer)
        {
            index = -1;
        }
        return spawnPointBehavior.GetHandPoint(NumPlayers(), index);
    }

    private int GetPlayerPositionIndex(PlayerOhHell player)
    {
        int playerListIndex = GetPlayerIndex(player);
        int localPlayerListIndex = GetPlayerIndex(GetLocalPlayer());
        int numPlayers = GetLocalPlayerList().Count;
        int playerIndexToUse = playerListIndex - localPlayerListIndex - 1; //index in spawnpoint arr
        if (playerIndexToUse < 0)
        {
            playerIndexToUse = playerIndexToUse + numPlayers;
        }
        return playerIndexToUse;
    }

    private List<PlayerOhHell> GetLocalPlayerList()
    {
        List <PlayerOhHell> localList = new List<PlayerOhHell>();
        foreach(uint id in playerIds)
        {
            localList.Add(GetPlayerByNetId(id));
        }
        return localList;
    }

    private PlayerOhHell GetPlayerByNetId(uint targetID)
    {
        if (NetworkIdentity.spawned.TryGetValue(targetID, out NetworkIdentity identity))
        {
            return identity.gameObject.GetComponent<PlayerOhHell>();
        }
        throw new Exception("failed to get player with ID: " + targetID);
    }

    public void BeginGame()
    {
        foreach (PlayerOhHell player in players)
        {
            player.RpcInitializeUI(netId);
        }
        StartRound();
    }

    private void InitiateScoreboard(bool isHalfTime)
    {
        foreach (PlayerOhHell player in players)
        {
            player.RpcInitiateScoreboard(isHalfTime);
        }
    }

    private void InitiateBidPhase()
    {
        foreach (PlayerOhHell player in players)
        {
            player.RpcInitiateBidPhase();
        }
    }

    private void EndBidPhase()
    {
        foreach (PlayerOhHell player in players)
        {
            player.RpcEndBidPhase();
        }
    }

    private void StartPostRound()
    {
        foreach (PlayerOhHell player in players)
        {
            player.RpcStartPostRound();
        }
    }

    private void StartRound()
    {
        if (isServer)
        {
            Deck deck = new Deck();
            TricksPlayedThisRound = 0;
            if (RoundCardNumDecreasing)
            {
                if(CurrentRoundCardNum == 1)
                {
                    IsIndianRound = true;
                }
                else
                {
                    CurrentRoundCardNum--;
                }
            }
            else
            {
                CurrentRoundCardNum++;
                if (CurrentRoundCardNum > (MaxHand-1)) 
                {
                    RoundCardNumDecreasing = true;
                }
            }
            foreach (PlayerOhHell player in players)
            {
                player.RpcRoundStart(deck.DrawHand(CurrentRoundCardNum), IsIndianRound);
            }
            TrumpCard = deck.DrawCard();

        }

        this.StartCoroutine(() =>
        {
            StartRound2();
        }, 0.1f);
    }

    private void StartRound2()
    {
        if (isServer)
        {
            foreach (PlayerOhHell player in players)
            {
                player.IsMyTurn = false;
                player.CurrentRoundBid = -1;
                player.RpcSendSelfUIUpdate();
            }
            ShowOtherBids = false;
        }

    }

    //called from a PlayerOhHell Command to run on server
    public void BidChosen(PlayerOhHell pl)
    {
        bool allPlayersBid = true;
        foreach(PlayerOhHell player in players)
        {
            if(player.CurrentRoundBid == -1)
            {
                allPlayersBid = false;
            }
        }
        if (allPlayersBid)
        {
            this.StartCoroutine(() =>
            {
                InitiateBidPhase();
            }, 0.1f);
           
            this.StartCoroutine(() =>
            {
                StartRoundAfterBids();
            }, 6.5f);
        }
        UpdateAllPlayerUIs();
    }

    private void StartRoundAfterBids()
    {
        EndBidPhase();
        ShowOtherBids = true;
        currentTurnPlayerIndex = roundFirstLeader;
        players[currentTurnPlayerIndex].IsMyTurn = true;
        //get ShowOtherBids change
        this.StartCoroutine(() =>
        {
            UpdateAllPlayerUIs();
        }, 0.1f);
    }

    private void UpdateAllPlayerUIs()
    {
        foreach (PlayerOhHell player in players)
        {
            player.RpcSendSelfUIUpdate();
        }
    }

    //called from a PlayerOhHell Command to run on server
    public void CardChosen(PlayerOhHell pl, Card card)
    {
        if (!isServer)
        {
            return;
        }
        int oldTurnPlayerIndex = currentTurnPlayerIndex;
        int newTurnPlayerIndex = currentTurnPlayerIndex;
        bool trickEnd = false, roundEnd = false;

        cardsInCenter.Add(card);
        if (GetLeadingSuit() == null)
        {
            SetLeadingSuit(card.Suit);
        }
        
        if (card.IsStronger(TrickWinningCard, GetLeadingSuit(), GetTrumpSuit()))
        {
            TrickWinningCard = card;
            TrickWinningPlayer = pl;
        }


        if (cardsInCenter.Count < players.Count)
        {
            //advance turn
            currentTurnPlayerIndex++;
            if (currentTurnPlayerIndex >= players.Count)
            {
                currentTurnPlayerIndex = 0;
            }
            newTurnPlayerIndex = currentTurnPlayerIndex;
        }
        else
        {
            TricksPlayedThisRound++;
            currentTurnPlayerIndex = GetPlayerIndex(TrickWinningPlayer);
            TrickWinningPlayer.TricksThisRound++;
            newTurnPlayerIndex = currentTurnPlayerIndex;
            trickWinningPlayerId = TrickWinningPlayer.netId;
            TrickWinningPlayer = null;
            TrickWinningCard = null;
            SetLeadingSuit(null);
            trickEnd = true;
            cardsInCenter.Clear();
            if(TricksPlayedThisRound >= CurrentRoundCardNum)
            {
                roundEnd = true;
            }
        }

        float delay = 0.1f;
        if (trickEnd)
        {
            players[oldTurnPlayerIndex].IsMyTurn = false;
            delay = 3.8f; //time for 'X won the trick to show' before clearing middle cards
        }
        this.StartCoroutine(() =>
        {
            UpdateAllPlayerUIs();
        }, 0.01f);

        this.StartCoroutine(() =>
        {
            CardChosen2(oldTurnPlayerIndex, newTurnPlayerIndex, trickEnd, roundEnd);
        }, delay);
    }

    private void CardChosen2(int oldTurnPlayerIndex, int newTurnPlayerIndex, bool trickEnd, bool roundEnd)
    {
        players[oldTurnPlayerIndex].IsMyTurn = false;
        if (trickEnd)
        {
            trickWinningPlayerId = 999999;
            foreach (PlayerOhHell player in players)
            {
                player.RpcTrickEnd();
            }

            this.StartCoroutine(() =>
            {
                UpdateAllPlayerUIs();
            }, 0.1f);
        }
        if (roundEnd)
        {
            foreach (PlayerOhHell player in players)
            {
                int roundScore = player.TricksThisRound;
                if (player.TricksThisRound == player.CurrentRoundBid)
                {
                    roundScore += 10;
                }
                player.ScoreLastRound = roundScore;
                player.CurrentScore += roundScore;

                player.TricksThisRound = 0;
                player.CurrentRoundBid = -1;
            }
            roundFirstLeader++;
            if (roundFirstLeader >= NumPlayers())
            {
                roundFirstLeader = 0;
            }

            float timeForRoundScores = 4.0f;
            float roundStartDelay = timeForRoundScores;
            float halfTimeLength = NumPlayers() + 5.0f;

            bool doHalfTime = CurrentRoundCardNum == MaxHand;
            bool startAnotherRound = !IsIndianRound;
            bool doGameEnd = !startAnotherRound;
            if (doHalfTime)
            {
                this.StartCoroutine(() =>
                {
                    InitiateScoreboard(true);
                }, timeForRoundScores);
                roundStartDelay += halfTimeLength;
            }
            if (doGameEnd)
            {
                //game end
                this.StartCoroutine(() =>
                {
                    InitiateScoreboard(false);
                }, timeForRoundScores);
            }
            this.StartCoroutine(() =>
            {
                StartPostRound();
            }, 0.2f);

            if (startAnotherRound)
            {
                this.StartCoroutine(() =>
                {
                    StartRound();
                }, roundStartDelay);
            }
        }
        else
        {
            players[newTurnPlayerIndex].IsMyTurn = true;
        }
    }

    public string GetTrickLeaderName()
    {
        List<PlayerOhHell> localList = GetLocalPlayerList();
        PlayerOhHell player = localList[roundFirstLeader];
        if (player.isLocalPlayer)
        {
            return "You";
        }
        return localList[roundFirstLeader].PlayerName;
    }

    public bool DidAnyBidFire()
    {
        List<PlayerOhHell> localList = GetLocalPlayerList();
        foreach (PlayerOhHell p in localList)
        {
            if(p.CurrentRoundBid >= 4)
            {
                return true;
            }
        }
        return false;
    }

    public bool PlayerIsTrickWinner(PlayerOhHell pl)
    {
        return pl.netId == trickWinningPlayerId;
    }

    public string GetTrickWinnerName()
    {
        List<PlayerOhHell> localList = GetLocalPlayerList();
        PlayerOhHell winner = null;
        
        foreach(PlayerOhHell p in localList)
        {
            if(p.netId == trickWinningPlayerId)
            {
                winner = p;
            }
        }
        if(winner == null)
        {
            return null;
        }
        
        if (winner.isLocalPlayer)
        {
            return "You";
        }
        return winner.PlayerName;
    }

    public List<ScorePair> GetScores()
    {
        List<ScorePair> scoreList = new List<ScorePair>();
        List<PlayerOhHell> localList = GetLocalPlayerList();
        foreach(PlayerOhHell pl in localList)
        {
            scoreList.Add(new ScorePair(pl.PlayerName, pl.CurrentScore));
        }
        scoreList.Sort();
        return scoreList;
    }

    public CardSuit? GetTrumpSuit()
    {
        return TrumpCard?.Suit;
    }

    public CardSuit? GetLeadingSuit()
    {
        if(LeadingSuit == -1)
        {
            return null;
        }
        return (CardSuit)LeadingSuit;
    }

    private void SetLeadingSuit(CardSuit? suit)
    {
        if (suit == null)
        {
            LeadingSuit = -1;
        }
        else
        {
            LeadingSuit = (int)suit;
        }
    }
}

public static class MonoBehaviourExtension
{
    public static Coroutine StartCoroutine(this MonoBehaviour behaviour, System.Action action, float delay)
    {
        return behaviour.StartCoroutine(WaitAndDo(delay, action));
    }

    private static IEnumerator WaitAndDo(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
