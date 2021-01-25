using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftMove : MonoBehaviour
{
    [SerializeField] float verticalDistance;
    [SerializeField] float horizontalDistance;
    [Range(0, 1)]
    [SerializeField] float moveSpeed;

   
    Vector3 endPos1;
    Vector3 endPos2;
    Vector3 currentDestination;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 originPos = transform.position;
        endPos1.Set(originPos.x, originPos.y + verticalDistance, originPos.z + horizontalDistance);
        endPos2.Set(originPos.x, originPos.y - verticalDistance, originPos.z - horizontalDistance);
        currentDestination = endPos1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.isPause)
        {
            if ((transform.position - endPos1).sqrMagnitude <= 0.1f)
                currentDestination = endPos2;
            if ((transform.position - endPos2).sqrMagnitude <= 0.1f)
                currentDestination = endPos1;

            transform.position = Vector3.MoveTowards(transform.position, currentDestination, moveSpeed);
        }
    }
   
}
