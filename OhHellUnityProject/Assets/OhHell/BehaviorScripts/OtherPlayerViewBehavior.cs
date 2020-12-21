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

    public static void ThrowCard(GameObject card, Vector3 targetPoint)
    {
        MoveToPoint moveToPoint = card.AddComponent<MoveToPoint>();
        moveToPoint.targetPoint = targetPoint;
        moveToPoint.speed = 2.0f;
    }
    public void PlayCard(Card card)
    {
        ThrownCard = Instantiate(CardPrefab);
        ThrownCard.GetComponent<CardVisualBehavior>().SetCard((card));
        ThrownCard.transform.position = CardSpawnPoint.transform.position;
        ThrowCard(ThrownCard, CardTargetPoint.transform.position);
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
