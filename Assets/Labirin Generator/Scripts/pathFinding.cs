using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class pathFinding : MonoBehaviour
{
    //public Transform tujuan;
    private NavMeshPath path;
    private NavMeshAgent agent;
    private float elapsed = 0.0f;
    private GameObject tujuan;
    private float jarak;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        elapsed = 0.0f;
        
        //agent.destination = tujuan.transform.position;
        //jarak = agent.remainingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        tujuan = GameObject.FindGameObjectWithTag("Finish");
        elapsed += Time.deltaTime;
        agent.destination = tujuan.transform.position;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            NavMesh.CalculatePath(transform.position, tujuan.transform.position, NavMesh.AllAreas, path);
            
        }
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

        //Debug.Log("Elapsed = " + elapsed);
        //Debug.Log(agent.destination + " " + path.status + " " + tujuan.transform.position);
    }
}