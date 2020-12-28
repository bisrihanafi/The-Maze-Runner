using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GenerateObject : MonoBehaviourPunCallbacks
{
    #region Public Fields
    [Tooltip("Object anything")]
    public GameObject objectIni;
    #endregion
    
    private void Start()
    {
        if (objectIni == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> Bola Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            PhotonNetwork.InstantiateSceneObject(this.objectIni.name, new Vector3(0f, 30f, 0f), Quaternion.identity, 0);


        }
    }

}
