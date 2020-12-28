using UnityEngine;
using System.Collections;

public class RoundScoreMakerBehavior : MonoBehaviour
{
    public GameObject RoundScorePrefab;
    public bool forLocal;

    public GameObject MakeScoreObject(int points, Vector3 position)
    {
        GameObject newOb = Instantiate(RoundScorePrefab);
        newOb.GetComponent<RoundScoreBehavior>().Setup(points, forLocal);
        newOb.transform.position = position;
        return newOb;
    }
}
