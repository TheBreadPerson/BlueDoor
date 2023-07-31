using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSway : MonoBehaviour
{
    public bool isGun = true;
    [Header("Sway")]
    float swayM;
    public float swayMultiplier, aimSwayMult;
    public float swaySmooth;

    Gun gun;

    // Start is called before the first frame update
    void Start()
    {
        if(isGun)
        {
            gun = GetComponentInChildren<Gun>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Sway();
        if (isGun)
        {
            if (gun.aiming)
            {
                swayM = aimSwayMult;
            }
            else
            {
                swayM = swayMultiplier;
            }
        }
        else
        {
            swayM = swayMultiplier;
        }
        
    }

    private void Sway()
    {
        float mx = Input.GetAxisRaw("Mouse X") * swayM;
        float my = Input.GetAxisRaw("Mouse Y") * swayM;

        Quaternion xRotation = Quaternion.AngleAxis(-my, Vector3.right);
        Quaternion yRotation = Quaternion.AngleAxis(mx, Vector3.up);

        Quaternion targetRot = xRotation * yRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, swaySmooth * Time.deltaTime);
    }
}
