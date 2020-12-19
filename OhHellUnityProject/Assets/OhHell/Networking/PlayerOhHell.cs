using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOhHell : NetworkBehaviour
{
    public float speed = 30;
    public Rigidbody2D rigidbody2d;
    public GameObject playerViewSelfPrefab;
    public GameObject playerViewOtherPrefab;
    private GameObject handUI;
    private GameManager gameManager;
    private OtherPlayerViewBehavior otherPlayerViewBehavior;
    void FixedUpdate()
    {
    }

    [ClientRpc]
    public void SetHand(List<Card> hand)
    {
        if (isLocalPlayer)
        {
            handUI.GetComponent<CardHandBehavior>().SetCards(hand);
        }
    }
    [ClientRpc]
    public void InitializeUI(int amount)
    {
        if (isLocalPlayer)
        {
            GameObject myPlayerUI = Instantiate(playerViewSelfPrefab);
            GameObject otherPlayerUI = Instantiate(playerViewOtherPrefab);
            otherPlayerViewBehavior = otherPlayerUI.GetComponent<OtherPlayerViewBehavior>();
            handUI = myPlayerUI;
            CardHandBehavior cardHandBehavior = myPlayerUI.GetComponent<CardHandBehavior>();
            cardHandBehavior.ClickCardEvent.AddListener(OnCardChosen);
        }
        else
        {
          //  GameObject ob = GameObject.Instantiate(playerViewOtherPrefab);
           // Text text = GameObject.Find("PVOtext").GetComponent<Text>();
           // text.text = "Other player is" + amount;
       //    (NetworkManager. as NetworkManagerOhHell).CommandOne();
        }
    }

    private void OnCardChosen(Card card)
    {
        CommandOne(card);
    }

    [Command]
    public void CommandOne(Card card)
    {
        //Debug.Log("command called with card: " + card.ToString());
        gameManager.CardChosen(this, card);
    }
    public void SetGameManager(GameManager gameManagerParam)
    {
        gameManager = gameManagerParam;
    }

    [ClientRpc]
    public void Display(string message)
    {
        if (isLocalPlayer)
        {
            otherPlayerViewBehavior.messageDisplayer.SetMessage(message);

        }
    }
}
