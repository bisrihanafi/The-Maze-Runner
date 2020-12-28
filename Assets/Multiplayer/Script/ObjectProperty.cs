using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;
using Photon.Pun;
using Photon.Realtime;

namespace Hanafi
{
    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class ObjectProperty : MonoBehaviourPunCallbacks, IPunObservable
    {
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
               
                //stream.SendNext(gameObject.name);
                //stream.SendNext(gameObject.tag);
                //stream.SendNext(gameObject.transform.position);
                
            }
            else
            {
                // Network player, receive data

                //gameObject.name = (string)stream.ReceiveNext();
                //gameObject.tag = (string)stream.ReceiveNext();
                //gameObject.transform.position= (Vector3)stream.ReceiveNext();
                //Debug.Log(gameObject.name + " Delta Time "+ PhotonNetwork.ServerTimestamp);
            }
        }
    }

}