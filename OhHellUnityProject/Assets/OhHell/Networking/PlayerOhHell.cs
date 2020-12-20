using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOhHell : NetworkBehaviour
{
    public GameObject playerViewSelfPrefab;
    public GameObject playerViewOtherPrefab;

    [SyncVar(hook = nameof(SetPlayerName))]
    public string PlayerName;

    [SyncVar]
    public uint gameManagerNetId;

    [SyncVar(hook = nameof(SetIsMyTurn))]
    public bool IsMyTurn;

    private OtherPlayerViewBehavior otherPlayerViewBehavior;
    private PlayerSelfViewBehavior playerSelfViewBehavior;

    void FixedUpdate()
    {
    }
    void SetPlayerName(string oldColor, string newColor)
    {
        PlayerName = newColor;
        if (!isLocalPlayer && otherPlayerViewBehavior != null)
        {
            otherPlayerViewBehavior.UpdatePlayerName(PlayerName);
        }
    }
    void SetIsMyTurn(bool oldColor, bool newColor)
    {
        IsMyTurn = newColor;
        if (isLocalPlayer && playerSelfViewBehavior != null)
        {
            playerSelfViewBehavior.UpdateTurnUI(IsMyTurn);
        }
    }
    //On client
    [ClientRpc]
    public void RoundStart(List<Card> hand)
    {
        if (isLocalPlayer)
        {
            playerSelfViewBehavior.OnNewRound(hand);
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
    public void InitializeUI(uint amount)
    {
        gameManagerNetId =  amount;
        if (isLocalPlayer)
        {
            GameObject myPlayerUI = Instantiate(playerViewSelfPrefab); ;
            playerSelfViewBehavior = myPlayerUI.GetComponent<PlayerSelfViewBehavior>();
            playerSelfViewBehavior.Initialize();
            playerSelfViewBehavior.UpdateTurnUI(IsMyTurn);
            playerSelfViewBehavior.CardSelectedEvent.AddListener(OnCardChosen);

            // GameObject otherPlayerUI = Instantiate(playerViewOtherPrefab);
            // otherPlayerViewBehavior = otherPlayerUI.GetComponent<OtherPlayerViewBehavior>();

        }
        else
        {
            GameObject ob = Instantiate(playerViewOtherPrefab);
            otherPlayerViewBehavior = ob.GetComponent<OtherPlayerViewBehavior>();
            otherPlayerViewBehavior.UpdatePlayerName(PlayerName);
            ob.transform.position = GetGameManager().GetPlayerPosition(this);
           // Text text = GameObject.Find("PVOtext").GetComponent<Text>();
           // text.text = "Other player is" + amount;
           //    (NetworkManager. as NetworkManagerOhHell).CommandOne();
        }
        if(1 == netId)
        {
            Startup();
        }
    }
    [Command]
    private void Startup()
    {
        GetGameManager().StartUp();
    }

    private void OnCardChosen(GameObject sourceObj, Card card)
    {
        CmdCardChosen(card);
    }

    


    //SERVER
    [Command]
    public void CmdCardChosen(Card card)
    {
        GetGameManager().CardChosen(this, card);
        PlayCard(card); //send to other clients
    }
    private GameManager GetGameManager()
    {
        if (NetworkIdentity.spawned.TryGetValue(gameManagerNetId, out NetworkIdentity identity))
        {
            return identity.gameObject.GetComponent<GameManager>();
        }
        throw new Exception("failed to get gamemanager using ID + "+ gameManagerNetId);
    }

}
