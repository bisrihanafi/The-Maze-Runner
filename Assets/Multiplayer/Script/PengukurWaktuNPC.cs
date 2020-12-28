using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;

namespace Hanafi
{
    public class PengukurWaktuNPC : MonoBehaviourPunCallbacks, IPunObservable
    {
        private uint count_delay = 0;
        Text status_ping;
        long ping = 0;

        void Start() {
            status_ping = GameObject.Find("Ping").GetComponent<Text>();
        }
        private void FixedUpdate()
        {
            ping = long.Parse(status_ping.text);
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
            }
            else
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
                count_delay++;
                string fill_log = "Registered NPC "+this.gameObject.name+" " + count_delay + " " + ((int)(lag * 1000f)) + " " + ping;
                Debug.Log(fill_log);
            }
        }
    }
}
