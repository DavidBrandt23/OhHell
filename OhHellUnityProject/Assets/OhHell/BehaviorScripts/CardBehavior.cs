using UnityEngine;
using System.Collections;

public class CardBehavior : MonoBehaviour
{
   private Card myCard;
    public CardEvent CardSelectedEvent ;
    public bool DefaultOn;
    private void Awake()
    {
        CardSelectedEvent = new CardEvent();

        if (DefaultOn)
        {
            SetCard(new Card(CardSuit.Spade, 13));
        }

      //  GetComponent<SelectableObjectBehavior>().clicked.AddListener(onCardSelected);
    }
    public void onCardSelected()
    {
        Debug.Log("on card sel");
        CardSelectedEvent.Invoke(gameObject, myCard);
    }
    public void SetCard(Card card)
    {
        myCard = card;
        GetComponent<CardVisualBehavior>().SetCard(myCard);
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
