using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerBehavior : MonoBehaviour
{
    public GameObject CardSpawnPoint;
    public GameObject CardTargetPoint;
    public GameObject CardPrefab;

    private GameObject playedCard;

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
        playedCard = Instantiate(CardPrefab);
        playedCard.GetComponent<CardVisualBehavior>().SetCard((card));
        playedCard.transform.position = CardSpawnPoint.transform.position;
        MoveToPoint moveToPoint = playedCard.AddComponent<MoveToPoint>();
        moveToPoint.targetPoint = CardTargetPoint.transform.position;
        moveToPoint.speed = 1.0f;
    }
}
