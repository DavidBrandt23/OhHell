﻿using Mirror;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class NetworkManagerOhHell : NetworkManager
{
    private List<PlayerOhHell> players;

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
        player.GetComponent<PlayerOhHell>().InitializeUI(1);
        players.Add(player.GetComponent<PlayerOhHell>());
        if(numPlayers == 2)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        GameObject gameManagerOb = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameManager"));
        NetworkServer.Spawn(gameManagerOb);
        GameManager gameManger = gameManagerOb.GetComponent<GameManager>();
        Deck deck = new Deck();
        foreach (PlayerOhHell player in players)
        {
            player.SetHand(deck.DrawHand(6));
            player.SetGameManager(gameManger);

            gameManger.players.Add(player);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}

