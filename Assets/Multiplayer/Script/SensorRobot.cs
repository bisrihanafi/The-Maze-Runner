using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SensorRobot : MonoBehaviour
{
    [SerializeField] private GameObject suriken;
    private Animator surikenanim;
    float i;
    // Start is called before the first frame update
    void Start()
    {
        surikenanim = suriken.GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        i = GetComponent<AIDestinationSetter>().remainingToTarget;
        
        if (i < 3f)
        {
            surikenanim.SetBool("OpenSuriken", true);
        }
        else {
            surikenanim.SetBool("OpenSuriken", false);
        }
    }
}
