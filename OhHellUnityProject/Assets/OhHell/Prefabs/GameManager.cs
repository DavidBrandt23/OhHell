using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public List<PlayerOhHell> players;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CardChosen(PlayerOhHell pl, Card card)
    {
        foreach (PlayerOhHell player in players)
        {
            player.Display("Player " + pl.netId + " played a card: " + card.ToString());
        }
    }
}
