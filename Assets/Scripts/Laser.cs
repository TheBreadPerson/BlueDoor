using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    RaycastHit hit;
    public LayerMask canLaser;
    public float range;
    public LineRenderer lineRenderer;
    public Transform CamT, AimT;
    public Gun gunScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gunScript.aiming)
        {
            if (Physics.Raycast(AimT.position, transform.TransformDirection(Vector3.forward), out hit, range, canLaser))
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, transform.position);
            }
        }
        else
        {
            if (Physics.Raycast(CamT.position, transform.TransformDirection(Vector3.forward), out hit, range, canLaser))
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, transform.position);
            }
        }

        
    }
}
