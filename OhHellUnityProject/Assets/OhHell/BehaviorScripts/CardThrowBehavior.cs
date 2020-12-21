using UnityEngine;
using System.Collections;

public class CardThrowBehavior : MonoBehaviour
{

    public GameObject CardPrefab;
    public GameObject ThrowCard(Card card, Vector3 spawnPoint, Vector3 targetPoint)
    {
        GameObject newObj = Instantiate(CardPrefab);
        newObj.GetComponent<CardVisualBehavior>().SetCard((card));
        newObj.transform.position = spawnPoint;

        MoveToPoint moveToPoint = newObj.AddComponent<MoveToPoint>();
        moveToPoint.targetPoint = targetPoint;
        moveToPoint.speed = 2.0f;
        return newObj;
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
