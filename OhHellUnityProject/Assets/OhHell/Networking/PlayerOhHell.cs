using Mirror;
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
            GameObject ob = GameObject.Instantiate(playerViewSelfPrefab);
            handUI = ob;

        }
        else
        {
          //  GameObject ob = GameObject.Instantiate(playerViewOtherPrefab);
           // Text text = GameObject.Find("PVOtext").GetComponent<Text>();
           // text.text = "Other player is" + amount;
       //    (NetworkManager. as NetworkManagerOhHell).CommandOne();
        }
    }
}
