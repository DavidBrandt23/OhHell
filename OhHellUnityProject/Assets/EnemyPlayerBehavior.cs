using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerBehavior : MonoBehaviour
{
    public GameObject CardSpawnPoint;
    public GameObject CardTargetPoint;
    public GameObject CardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp("space"))
        {
            PlayCard(new Card(CardSuit.Heart, 11));
        }
    }
    
    private void PlayCard(Card card)
    {
        GameObject cardOb = Instantiate(CardPrefab);
        cardOb.GetComponent<CardVisualBehavior>().SetCard((card));
        cardOb.transform.position = CardSpawnPoint.transform.position;
        MoveToPoint moveToPoint = cardOb.AddComponent<MoveToPoint>();
        moveToPoint.targetPoint = CardTargetPoint.transform.position;
        moveToPoint.speed = 1.0f;
    }
}
