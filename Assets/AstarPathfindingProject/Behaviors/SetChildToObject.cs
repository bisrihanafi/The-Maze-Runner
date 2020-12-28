using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetChildToObject : MonoBehaviour
{
    public string target;
    // Start is called before the first frame update
    void Awake()
    {
        this.transform.SetParent(GameObject.Find(target).GetComponent<Transform>(), false);
    }

    
}
