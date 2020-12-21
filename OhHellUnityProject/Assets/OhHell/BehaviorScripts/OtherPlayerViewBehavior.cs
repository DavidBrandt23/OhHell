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
    public Text ScoreText;
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
    public void UpdateScoreUI(int scor)
    {
        ScoreText.text = scor.ToString();
    }
    
    public void PlayCard(Card card)
    {
        ThrownCard = GetComponent<CardThrowBehavior>().ThrowCard(card, CardSpawnPoint.transform.position, CardTargetPoint.transform.position);
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
