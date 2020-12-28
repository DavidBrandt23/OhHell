using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BidSelectedEvent : UnityEvent<int>
{
}

public class BidButtonBehavior : MonoBehaviour
{
    public BidSelectedEvent BidSelectedEvent;
    public BidVisualBehavior bidVisualBehavior;

    private int bidValue;

    private void Awake()
    {
        BidSelectedEvent = new BidSelectedEvent();
    }

    public void SetBidValue(int val)
    {
        bidValue = val;
        if(val > 6)
        {
            throw new System.Exception("tried to set bid button value > 6");
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        bidVisualBehavior.UpdateUI(bidValue);
    }

    public void RaiseBidSelectedEvent()
    {
        Debug.Log("raise bid " + bidValue);
        BidSelectedEvent.Invoke(bidValue);
    }
}
