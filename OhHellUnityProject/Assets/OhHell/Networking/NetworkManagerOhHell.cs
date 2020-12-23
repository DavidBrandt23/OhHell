using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[AddComponentMenu("")]
public class NetworkManagerOhHell : NetworkManager
{
    //private List<PlayerOhHell> players;
    private GameManager gameManager;
    private GameObject lobbyUI;
    public TMP_InputField IPField;
    public TMP_InputField PlayerNameField;
    public GameObject MainMenuObject;
    public string localPlayerName;
    public override void Start()
    {
        base.Start();
       // players = new List<PlayerOhHell>();
        //GameObject gameManagerOb = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameManager"));
        //NetworkServer.Spawn(gameManagerOb);
        //gameManager = gameManagerOb.GetComponent<GameManager>();
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if(numPlayers == 0)
        {
            GameObject gameManagerOb = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameManager"));
            NetworkServer.Spawn(gameManagerOb);
            gameManager = gameManagerOb.GetComponent<GameManager>();

            lobbyUI = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "LobbyUI"));
            lobbyUI.GetComponent<LobbyUIBehavior>().StartEvent = new UnityEvent();
            lobbyUI.GetComponent<LobbyUIBehavior>().StartEvent.AddListener(StartGame);
            NetworkServer.Spawn(lobbyUI);
        }
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        PlayerOhHell newPlayerScript = player.GetComponent<PlayerOhHell>();

        gameManager.players.Add(newPlayerScript);
        gameManager.playerIds.Add(newPlayerScript.netId);

        newPlayerScript.GetInputPlayerName();


        Invoke("UpdateLobbyNames", 0.1f);
        
    }

    public void TryStartHost()
    {
        StartHost();
        localPlayerName = PlayerNameField.text;
        MainMenuObject.SetActive(false);
    }
    public void TryJoin()
    {
        networkAddress = IPField.text;
        localPlayerName = PlayerNameField.text;
        StartClient();
        MainMenuObject.SetActive(false);
    }

    private void StartGame()
    {
        Destroy(lobbyUI.gameObject);
        Invoke("StartGame2", 0.5f);
    }
    private void StartGame2()
    {
        gameManager.BeginGame();
    }

    public void UpdateLobbyNames()
    {
        lobbyUI.GetComponent<LobbyUIBehavior>().SetNames(gameManager.GetPlayerNameList());
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
        }
    }
}

