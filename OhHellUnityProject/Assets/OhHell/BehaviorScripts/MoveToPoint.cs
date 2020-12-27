using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveToPoint : MonoBehaviour
{
    public Vector3 targetPoint;
    public float speed;
    UnityEvent OnReachTargetEvent;
    bool eventRaised;
    public Transform TargetTransform; //has pri

    private void Awake()
    {
        OnReachTargetEvent = new UnityEvent();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
