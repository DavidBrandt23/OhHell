using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncListUInt : SyncList<uint> { }
public class GameManager : NetworkBehaviour
{
    public List<PlayerOhHell> players; //only works on server
    public SyncListUInt playerIds = new SyncListUInt();
    public List<GameObject> SpawnPoints;
    private int currentTurnPlayerIndex = 0;
    // Start is called before the first frame update
    public void Awake()
    {
        //players = new SyncList<PlayerOhHell>();
    }
    void Start()
    {
        
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

    public Vector3 GetPlayerPosition(PlayerOhHell player)
    {
        int playerListIndex = GetPlayerIndex(player);
        int localPlayerListIndex = GetPlayerIndex(GetLocalPlayer());
        int numPlayers = GetLocalPlayerList().Count;
        int playerIndexToUse = playerListIndex - localPlayerListIndex - 1; //index in spawnpoint arr
        if(playerIndexToUse < 0)
        {
            playerIndexToUse = playerIndexToUse + numPlayers;
        }
        return SpawnPoints[playerIndexToUse].transform.position;
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

    public void StartUp()
    {
        List<PlayerOhHell> localList = GetLocalPlayerList();
        for (int i = 0; i < localList.Count; i++)
        {
         //   localList[i].IsMyTurn = !localList[i].IsMyTurn;
        }
    }
    public void CardChosen(PlayerOhHell pl, Card card)
    {
        foreach (PlayerOhHell player in players)
        {
            player.Display("Player " + pl.netId + " played a card: " + card.ToString());
        }
        players[currentTurnPlayerIndex].IsMyTurn = false;
        currentTurnPlayerIndex++;
        if(currentTurnPlayerIndex >= players.Count)
        {
            currentTurnPlayerIndex = 0;
        }
        players[currentTurnPlayerIndex].IsMyTurn = true;
    }
}
