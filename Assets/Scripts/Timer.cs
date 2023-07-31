using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeOnLevel;
    public float maxTime;
    public PlayerMovement pm;
    public TextMeshProUGUI timeText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeOnLevel += Time.deltaTime;
        Display();

        if(timeOnLevel >= maxTime)
        {
            pm.Death();
        }
    }

    void Display()
    {
        timeText.text = Mathf.Round(timeOnLevel * 10) * .1f + "";
    }
}
