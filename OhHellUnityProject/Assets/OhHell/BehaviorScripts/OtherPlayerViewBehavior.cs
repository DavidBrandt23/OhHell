using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayerViewBehavior : MonoBehaviour
{
    public MessageDisplayer messageDisplayer;
    public GameObject CardSpawnPoint;
    public GameObject CardTargetPoint;
    public GameObject CardPrefab;
    public Text PlayerNameText;
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
    public void PlayCard(Card card)
    {
        GameObject playedCard;
        playedCard = Instantiate(CardPrefab);
        playedCard.GetComponent<CardVisualBehavior>().SetCard((card));
        playedCard.transform.position = CardSpawnPoint.transform.position;
        MoveToPoint moveToPoint = playedCard.AddComponent<MoveToPoint>();
        moveToPoint.targetPoint = CardTargetPoint.transform.position;
        moveToPoint.speed = 1.0f;
    }
}
