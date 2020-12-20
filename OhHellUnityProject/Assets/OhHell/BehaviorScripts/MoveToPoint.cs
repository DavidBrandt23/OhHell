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
        Vector3 currentPoint = transform.position;
        float distToTarget = (targetPoint - currentPoint).magnitude;
        Vector3 dirToTarget = (targetPoint - currentPoint).normalized;
        Vector3 vectorToMove = dirToTarget * speed;
        Vector3 newPosition = currentPoint + vectorToMove;
        if(distToTarget <= speed)
        {
            newPosition = targetPoint;
            OnReachTargetEvent.Invoke();
            eventRaised = true;
        }
        transform.position = newPosition;
    }
}
