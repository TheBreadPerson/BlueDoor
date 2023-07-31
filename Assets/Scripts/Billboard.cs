using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera mCam;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        mCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mCam.transform);
        transform.eulerAngles = transform.eulerAngles - offset;
    }
}
