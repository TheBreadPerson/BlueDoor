using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    private RaycastHit hit;
    public Transform shootPoint;
    public float gunRange;
    public LayerMask Shootable;
    private Color deafult;
    public Color enemyHit = Color.red;
    public RawImage crossH;

    // Start is called before the first frame update
    void Start()
    {
        deafult = crossH.color;
    }

    // Update is called once per frame
    void Update()
    {
        CheckEnemy();
    }

    void CheckEnemy()
    {
        
        
    }
}
