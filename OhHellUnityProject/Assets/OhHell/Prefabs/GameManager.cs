using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncListUInt : SyncList<uint> { }
public class GameManager : NetworkBehaviour
{
    public List<PlayerOhHell> players; //only works on server
    public SyncListUInt playerIds = new SyncListUInt();
    public SpawnPointBehavior spawnPointBehavior;
    private int MaxHand = 2; //6 for final

    [SyncVar]
    public Card TrumpCard;

    [SyncVar]
    private int currentTurnPlayerIndex = 0;

    [SyncVar]
    private int roundFirstLeader = 0;

    private List<Card> cardsInCenter;

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


    // Start is called before the first frame update
    public void Awake()
    {
        //players = new SyncList<PlayerOhHell>();
        cardsInCenter = new List<Card>();
        SetLeadingSuit(null);
        CurrentRoundCardNum = 0;
        TricksPlayedThisRound = 0;
    }
    void Start()
    {
        
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

    // Update is called once per frame
    void Update()
    {

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
        throw new System.Exception("failedto get ID: " + targetID);
    }

    private PlayerOhHell TrickWinningPlayer; //keep winner id in sync
    private Card TrickWinningCard;
    public void BeginGame()
    {

        foreach (PlayerOhHell player in players)
        {
            player.InitializeUI(netId);
        }
        StartRound();
    }
    private void InitiateScoreboard(bool isHalfTime)
    {
        foreach (PlayerOhHell player in players)
        {
            player.InitiateScoreboard(isHalfTime);
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
                player.RoundStart(deck.DrawHand(CurrentRoundCardNum), IsIndianRound);
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
                player.SendSelfUIUpdate();
            }
            ShowOtherBids = false;
        }

    }
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
            ShowOtherBids = true;
            currentTurnPlayerIndex = roundFirstLeader;
            players[currentTurnPlayerIndex].IsMyTurn = true;
            //get ShowOtherBids change
            this.StartCoroutine(() =>
            {
                UpdateAllPlayerUIs();
            }, 0.1f);
        }
    }

    private void UpdateAllPlayerUIs()
    {
        foreach (PlayerOhHell player in players)
        {
            player.SendSelfUIUpdate();
        }
    }

    public void CardChosen(PlayerOhHell pl, Card card)
    {
        if (!isServer)
        {
            return;
        }
        int oldTurnPlayerIndex = currentTurnPlayerIndex;
        int newTurnPlayerIndex = currentTurnPlayerIndex;
        bool trickEnd = false, roundEnd = false;

        foreach (PlayerOhHell player in players)
        {
            player.Display("Player " + pl.netId + " played a card: " + card.ToString());
        }

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
            delay = 4.0f; //time for 'X won the trick to show' before clearing middle cards
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
                player.TrickEnd();
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
                player.CurrentScore += player.TricksThisRound;
                if (player.TricksThisRound == player.CurrentRoundBid)
                {
                    player.CurrentScore += 10;
                }
                player.TricksThisRound = 0;
                player.CurrentRoundBid = -1;
            }
            roundFirstLeader++;
            if (roundFirstLeader >= NumPlayers())
            {
                roundFirstLeader = 0;
            }

            float roundStartDelay = 0.5f;
            float halfTimeLength = NumPlayers() + 5.0f;

            bool doHalfTime = CurrentRoundCardNum == MaxHand;
            bool startAnotherRound = !IsIndianRound;
            if (doHalfTime)
            {
                this.StartCoroutine(() =>
                {
                    InitiateScoreboard(true);
                }, 0.5f);
                roundStartDelay += halfTimeLength;
            }

            if (startAnotherRound)
            {
                this.StartCoroutine(() =>
                {
                    StartRound();
                }, roundStartDelay);
            }
            else
            {
                //gameend
                this.StartCoroutine(() =>
                {
                    InitiateScoreboard(false);
                }, 0.5f);
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
