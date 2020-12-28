using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BidVisualBehavior : MonoBehaviour
{
    public SpriteList HandSprites;

    public void UpdateUI(int bid)
    {
        GetComponent<SpriteRenderer>().sprite = HandSprites.GetSprite(bid);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
