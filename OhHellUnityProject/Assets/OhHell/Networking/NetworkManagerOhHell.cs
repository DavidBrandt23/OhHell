using Mirror;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class NetworkManagerOhHell : NetworkManager
{
    private List<PlayerOhHell> players;
    private GameManager gameManager;

    public override void Start()
    {
        base.Start();
        players = new List<PlayerOhHell>();
    }

    //[Command]
    //public void CommandOne()
    //{
    //    Debug.Log("command called");
    //}
    public override void OnServerAddPlayer(NetworkConnection conn)
    {

        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        players.Add(player.GetComponent<PlayerOhHell>());
        if(numPlayers == 3)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        GameObject gameManagerOb = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameManager"));
        NetworkServer.Spawn(gameManagerOb);
        gameManager = gameManagerOb.GetComponent<GameManager>();
        foreach (PlayerOhHell player in players)
        {
            gameManager.players.Add(player);
            gameManager.playerIds.Add(player.netId);
            player.PlayerName = "Player " + player.netId;
        }
        Invoke("StartGame2", 0.5f);
    }
    private void StartGame2()
    {
        Deck deck = new Deck();
        players[0].IsMyTurn = true;
        foreach (PlayerOhHell player in players)
        {
            //            player.gameManagerNetId = gameManger.netId;
            player.InitializeUI(gameManager.netId);
            player.RoundStart(deck.DrawHand(6));

        }
        Invoke("StartGame3", 0.5f);
    }

    private void StartGame3()
    {
        //gameManager.StartUp();
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}

