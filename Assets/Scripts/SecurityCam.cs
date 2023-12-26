using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCam : MonoBehaviour
{
    [Header("Horizontal")]
    [SerializeField] private float fov = 60f;
    [SerializeField] private int rayCount = 40;
    [SerializeField] private float range = 10f;
    [Header("Vertical")]
    [SerializeField] private float fovX = 60f;
    [SerializeField] private int rayCountX = 40;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < rayCount; i++)
        {
            float angle = (i / (float)rayCount) * fov - (fov/2f);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Ray r = new Ray(transform.position, dir);
            Debug.DrawRay(transform.position, dir * range, Color.red);
            Debug.Log(i);
        }
    }
}
