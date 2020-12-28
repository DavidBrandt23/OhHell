using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveToPoint : MonoBehaviour
{
    public Vector3 targetPoint;
    public float speed;
    UnityEvent OnReachTargetEvent;
    public Transform TargetTransform; //has priority

    private bool eventRaised;
    private void Awake()
    {
        OnReachTargetEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        if (eventRaised)
        {
            return;
        }
        Vector3 pointToTarget = targetPoint;
        if(TargetTransform != null)
        {
            pointToTarget = TargetTransform.position;
        }
        Vector3 currentPoint = transform.position;
        float distToTarget = (pointToTarget - currentPoint).magnitude;
        Vector3 dirToTarget = (pointToTarget - currentPoint).normalized;
        Vector3 vectorToMove = dirToTarget * speed;
        Vector3 newPosition = currentPoint + vectorToMove;
        if(distToTarget <= speed)
        {
            newPosition = pointToTarget;
            OnReachTargetEvent.Invoke();
            eventRaised = true;
        }
        transform.position = newPosition;
    }
}
