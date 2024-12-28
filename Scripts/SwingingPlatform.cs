using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwingingPlatform : MonoBehaviour
{
    public float swingSpeed = 1f;
    public float swingAngle = 60f; 

    private float currentAngle;

    void Update()
    {
       
        currentAngle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }
}

