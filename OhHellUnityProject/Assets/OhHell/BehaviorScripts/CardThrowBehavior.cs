using UnityEngine;
using System.Collections;

public class CardThrowBehavior : MonoBehaviour
{
    public GameObject CardPrefab;
    public AudioClip PlayCardSound;

    public GameObject ThrowCard(Card card, Vector3 spawnPoint, Vector3 targetPoint, bool noNoise = false)
    {
        GameObject newObj = Instantiate(CardPrefab);
        newObj.GetComponent<CardVisualBehavior>().SetCard((card));
        newObj.transform.position = spawnPoint;

        MoveToPoint moveToPoint = newObj.AddComponent<MoveToPoint>();
        moveToPoint.targetPoint = targetPoint;
        moveToPoint.speed = 2.0f;
        if (!noNoise)
        {
            GetComponent<AudioSource>().PlayOneShot(PlayCardSound);
        }
        return newObj;
    }
}
