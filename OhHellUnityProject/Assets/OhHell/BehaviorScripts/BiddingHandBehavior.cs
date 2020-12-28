using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BiddingHandBehavior : MonoBehaviour
{
    public bool DebugMode;

    public BidVisualBehavior bidVisualBehavior;
    public GameObject fireObject;
    public GameObject HandObject;
    public Animator bidAnimator;
    public UnityEvent animDoneEvent = new UnityEvent();

    private int bidNum;
    public void Awake()
    {
        bidAnimator.speed = 0;
        bidVisualBehavior.UpdateUI(0);
        if (DebugMode)
        {
            StartBid(3);
        }
    }
    public void StartBid(int bid)
    {
        bidNum = bid;
        float speed = 2.0f;
        bidAnimator.speed = speed;
        bidVisualBehavior.UpdateUI(0);
        float length = 0.6666f * 3.0f / speed; //anim is 40 frames at 60fps for .6666 secs

        this.StartCoroutine(() =>
        {
            StopBounce(bid);
        }, length);
    }

    private void StopBounce(int bid)
    {
        bidAnimator.speed = 0;
        bidVisualBehavior.UpdateUI(bid);
        animDoneEvent.Invoke();
        if(bid >= 4)
        {
            fireObject.SetActive(true);
        }
    }
}
