using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BidUIBehavior : MonoBehaviour
{
    public GameObject BidButtonPrefab;
    public BidSelectedEvent BidEvent;
    public Text LeaderText;
    public void Awake()
    {
        // SetupBidUI(3);
        //BidEvent = new BidSelectedEvent();
    }
    public void SetupBidUI(int maxBid, string leaderName)
    {
        BidEvent = new BidSelectedEvent();
        int numButtons = maxBid + 1;
        float cardSpacing = 17.0f;
        float firstCardOffset = -1 * (numButtons - 1) / 2.0f * cardSpacing;
        for(int i = 0; i <= maxBid; i++)
        {
            GameObject newButtonObj = Instantiate(BidButtonPrefab, transform);
            BidButtonBehavior bidButtonBehavior = newButtonObj.GetComponent<BidButtonBehavior>();
            bidButtonBehavior.SetBidValue(i);
            bidButtonBehavior.BidSelectedEvent = new BidSelectedEvent();
            bidButtonBehavior.BidSelectedEvent.AddListener(OnButtonClick);
            newButtonObj.transform.localPosition = new Vector3(firstCardOffset + i * cardSpacing, 0.0f, 0.0f);
        }
        LeaderText.text = leaderName + " will lead the first trick";
    }
    public void OnButtonClick(int bid)
    {
        BidEvent.Invoke(bid);
    }
}
