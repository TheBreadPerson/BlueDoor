using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    Transform dragObject;
    RaycastHit hit;
    bool dragging;
    bool follow;
    public float range;
    public float dragSpeed;
    public float distanceBeforeBreak;
    public LayerMask dragable;
    public Transform dragPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.E) && !dragging)
        {
            DragObject();
        }
        if(Input.GetKeyUp(KeyCode.E))
        {
            dragging = false;
            follow = false;
            dragObject = null;
        }
    }

    void DragObject()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hit, range, dragable))
        {
            follow = true;
            dragObject = hit.transform;
        }
        if(follow)
        {
            if (dragObject.gameObject.GetComponent<Rigidbody>())
            {
                Rigidbody rb;
                rb = dragObject.GetComponent<Rigidbody>();
                Vector3 direction = dragPoint.position - dragObject.position;
                float distance = Vector3.Distance(dragObject.position, dragPoint.position);
                rb.velocity = direction.normalized * dragSpeed / rb.mass * distance;
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, new Vector3(0f, 0f, 0f), dragSpeed/rb.mass * Time.deltaTime);

                if (distance >= distanceBeforeBreak)
                {
                    follow = false;
                    dragObject = null;
                }
            }
        }
    }

    void StopDrag()
    {
        // STOP DRAG
    }
}
