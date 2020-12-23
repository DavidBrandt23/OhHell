using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayerViewBehavior : MonoBehaviour
{
    public MessageDisplayer messageDisplayer;
    public GameObject CardSpawnPoint;
    public GameObject CardTargetPoint;
    public Text PlayerNameText;
    public Text TricksText;
    public Text ScoreText;
    public Text BidText;
    private GameObject IndianCard;
    private GameObject ThrownCard;
    public void UpdatePlayerName(string name)
    {
        PlayerNameText.text = name;
    }
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
        BidText.text = BidDisplayString(data.currentBid);
        TricksText.text = TricksDisplayString(data.currentTricks);
        ScoreText.text = ScoreDisplayString(data.currentScore);
    }

    public void OnNewRound(List<Card> hand, bool isIndianRound)
    {
        if (isIndianRound)
        {
            Card card = hand[0];
            Vector3 cardSpawn = CardSpawnPoint.transform.position;
            IndianCard = GetComponent<CardThrowBehavior>().ThrowCard(card, cardSpawn, cardSpawn);
            Vector3 curPos = IndianCard.transform.position;
            Vector3 newPos = new Vector3(curPos.x, curPos.y, -0.1f);
            IndianCard.transform.position = newPos;

        }
    }
    
    public void PlayCard(Card card)
    {
        if(IndianCard!= null)
        {
            Destroy(IndianCard);
            IndianCard = null;
        }
        ThrownCard = GetComponent<CardThrowBehavior>().ThrowCard(card, CardSpawnPoint.transform.position, CardTargetPoint.transform.position);
    }
    public static string BidDisplayString(int? bid)
    {
        return "Bid: " + NullIntToString(bid);
    }
    public static string TricksDisplayString(int? tricks)
    {
        return "Tricks: " + NullIntToString(tricks);
    }
    public static string ScoreDisplayString(int? score)
    {
        return "Score: " + NullIntToString(score);
    }
    private static string NullIntToString(int? input)
    {
        if (input == null)
        {
            input = -1;
        }
        string text = input.ToString();
        if (input < 0)
        {
            text = "";
        }
        return text;
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
