using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncListUInt : SyncList<uint> { }
public class GameManager : NetworkBehaviour
{
    public List<GameObject> PlayerSpawnPoints;
    public List<GameObject> CardSpawnPoints;

    public List<PlayerOhHell> players; //only works on server
    public SyncListUInt playerIds = new SyncListUInt();
    public SpawnPointBehavior spawnPointBehavior;

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
        //return PlayerSpawnPoints[GetPlayerPositionIndex(player)].transform.position;
    }
    public Vector3 GetPlayerCardTargetPosition(PlayerOhHell player)
    {
        return spawnPointBehavior.GetCardTarget(NumPlayers(), GetPlayerPositionIndex(player));
        //return CardSpawnPoints[GetPlayerPositionIndex(player)].transform.position;
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

    private PlayerOhHell TrickWinningPlayer;
    private Card TrickWinningCard;
    public void BeginGame()
    {

        foreach (PlayerOhHell player in players)
        {
            player.InitializeUI(netId);
        }
        StartRound();
    }
    private void StartRound()
    {
        if (isServer)
        {
            Deck deck = new Deck();
            TricksPlayedThisRound = 0;
            if (RoundCardNumDecreasing)
            {
                CurrentRoundCardNum--;
            }
            else
            {
                CurrentRoundCardNum++;
                if (CurrentRoundCardNum > 6)
                {
                    CurrentRoundCardNum = 5;
                    RoundCardNumDecreasing = true;
                }
            }
            foreach (PlayerOhHell player in players)
            {
                player.RoundStart(deck.DrawHand(CurrentRoundCardNum));
            }
            TrumpCard = deck.DrawCard();
           // currentTurnPlayerIndex = roundFirstLeader;
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
            //player.SendSelfUIUpdate();
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
            delay = 0.5f;
        }
        //don't update ismyturn until leading suit set
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
            foreach (PlayerOhHell player in players)
            {
                player.TrickEnd();
            }
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
            this.StartCoroutine(() =>
            {
                StartRound();
            }, 0.5f);
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
