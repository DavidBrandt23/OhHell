﻿using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[AddComponentMenu("")]
public class NetworkManagerOhHell : NetworkManager
{
    //private List<PlayerOhHell> players;
    private GameManager gameManager;
    private GameObject lobbyUI;

    public override void Start()
    {
        base.Start();
       // players = new List<PlayerOhHell>();
        //GameObject gameManagerOb = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameManager"));
        //NetworkServer.Spawn(gameManagerOb);
        //gameManager = gameManagerOb.GetComponent<GameManager>();
    }

    //[Command]
    //public void CommandOne()
    //{
    //    Debug.Log("command called");
    //}
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if(numPlayers == 0)
        {
            GameObject gameManagerOb = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameManager"));
            NetworkServer.Spawn(gameManagerOb);
            gameManager = gameManagerOb.GetComponent<GameManager>();
            //gameManager.StartMake();
            lobbyUI = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "LobbyUI"));
            lobbyUI.GetComponent<LobbyUIBehavior>().StartEvent = new UnityEvent();
            lobbyUI.GetComponent<LobbyUIBehavior>().StartEvent.AddListener(StartGame);
            NetworkServer.Spawn(lobbyUI);
        }
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        PlayerOhHell newPlayerScript = player.GetComponent<PlayerOhHell>();
        //players.Add(newPlayerScript);
        //GameObject.Find
        gameManager.players.Add(newPlayerScript);
        gameManager.playerIds.Add(newPlayerScript.netId);
        newPlayerScript.PlayerName = "Player " + (newPlayerScript.netId - 2);
        Invoke("UpdateLobbyNames", 0.1f);

        if (numPlayers == 3)
        {
            //StartGame();
        }
    }


    private void StartGame()
    {
        //foreach (PlayerOhHell player in players)
        //{
        // gameManager.players.Add(player);
        // gameManager.playerIds.Add(player.netId);
        //  player.PlayerName = "Player " + player.netId;
        //}
        Destroy(lobbyUI.gameObject);
        Invoke("StartGame2", 0.5f);
    }
    private void StartGame2()
    {
        gameManager.BeginGame();
    }

    private void UpdateLobbyNames()
    {
        lobbyUI.GetComponent<LobbyUIBehavior>().SetNames(gameManager.GetPlayerNameList());
        //gameManager.BeginGame();
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
    private void Update()
    {

        if (Input.GetKeyUp("space"))
        {
            //SceneManager.LoadScene("MainScene");
            // List<Card> newHand = GetRandomHand();
            // SetCards(newHand);
        }
    }
}

