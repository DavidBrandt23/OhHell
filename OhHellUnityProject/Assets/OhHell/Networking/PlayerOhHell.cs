using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOhHell : NetworkBehaviour
{
    public float speed = 30;
    public Rigidbody2D rigidbody2d;
    public GameObject test;

    // need to use FixedUpdate for rigidbody
    void FixedUpdate()
    {
        // only let the local player control the racket.
        // don't control other player's rackets
        if (isLocalPlayer)
            rigidbody2d.velocity = new Vector2(0, Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
    }

    [ClientRpc]
    public void CreateDialog(int amount)
    {
        Debug.Log("Took damage:" + amount);
        if (isLocalPlayer)
        {
            GameObject ob = GameObject.Instantiate(test, transform);

        }
      //  NetworkServer.Spawn(ob);
        //GameObject.Instantiate(test, transform);
        //GameObject.Instantiate(test);
    }
}
