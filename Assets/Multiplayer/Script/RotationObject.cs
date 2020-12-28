using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationObject : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private bool x=true,y=true,z=true;
    [SerializeField]
    float speed = -2f;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (x)
            transform.Rotate(speed, 0f, 0f);
        if (y)
            transform.Rotate(0f, speed, 0f);
        if (z)
            transform.Rotate(0f,0f, speed);
    }
}
