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
    private int bidNum;
    public UnityEvent animDoneEvent = new UnityEvent();
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
        bidAnimator.speed = 1.0f;
        bidVisualBehavior.UpdateUI(0);
        Invoke("StopBounce", 2.0f);
        //this.StartCoroutine(() =>
        //{
        //    StopBounce(bid);
        //}, 2.0f * 1);
    }
    // Start is called before the first frame update
    void Start()
    {
        //run anim 3 times
        //anim is 40 frames at 60fps for .667 secs
    }
    private void StopBounce()
    {
        int bid = bidNum;
        bidAnimator.speed = 0;
        bidVisualBehavior.UpdateUI(bid);
        animDoneEvent.Invoke();
        if(bid >= 4)
        {
            fireObject.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
