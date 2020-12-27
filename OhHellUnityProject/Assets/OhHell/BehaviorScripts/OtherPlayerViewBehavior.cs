using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayerViewBehavior : MonoBehaviour
{
    public GameObject CardSpawnPoint;
    public GameObject CardTargetPoint;
    public CardThrowBehavior cardThrowBehavior;

    private GameObject IndianCard;
    private GameObject ThrownCard;
    public PlayerInfoBoxBehavior playerInfoBox;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RefreshUI(DataNeededForPlayerUI data)
    {
        playerInfoBox.UpdateUI(data);
    }

    public void OnNewRound(List<Card> hand, bool isIndianRound)
    {
        if (isIndianRound)
        {
            Card card = hand[0];
            Vector3 cardSpawn = CardSpawnPoint.transform.position;
            Vector3 higher = new Vector3(cardSpawn.x, cardSpawn.y, cardSpawn.z - 0.6f);
            IndianCard = cardThrowBehavior.ThrowCard(card, higher, higher, true);
           // Vector3 curPos = IndianCard.transform.position;
           // Vector3 newPos = new Vector3(curPos.x, curPos.y, curPos.z - 1.0f);
           /// IndianCard.transform.position = newPos;

        }
    }
    
    public void PlayCard(Card card)
    {
        if(IndianCard != null)
        {
            Destroy(IndianCard);
            IndianCard = null;
        }
        ThrownCard = cardThrowBehavior.ThrowCard(card, CardSpawnPoint.transform.position, CardTargetPoint.transform.position);
    }


    public void TrickEnd()
    {
        if(ThrownCard != null)
        {
            Destroy(ThrownCard);
            ThrownCard = null;
        }
    }
}
