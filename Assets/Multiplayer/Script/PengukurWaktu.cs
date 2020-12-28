using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Photon.Pun;
using System;

namespace Hanafi
{
    public class PengukurWaktu : MonoBehaviour
    {
        float time_start;
        float target_curent;
        AIDestinationSetter aIdestination;
        int debugMode;
        bool sampai;
        byte now_target_num;
        // Start is called before the first frame update
        void Start()
        {
            debugMode = PlayerPrefs.GetInt("DebugerMode");
            time_start = PhotonNetwork.ServerTimestamp;
            aIdestination = gameObject.GetComponent<AIDestinationSetter>();
            sampai = false;
            now_target_num = 0;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (debugMode == 1)
            {
                if (now_target_num != aIdestination.numberTarget) {
                    time_start = PhotonNetwork.ServerTimestamp;
                    now_target_num = aIdestination.numberTarget;
                }
                target_curent = aIdestination.remainingToTarget;
                if (target_curent < 1 && sampai==false)
                {
                    Debug.Log(gameObject.name+" need time "+Math.Abs((time_start - PhotonNetwork.ServerTimestamp) / 1000)+"s ");
                    sampai = true;
                }
                if (target_curent > 1) {
                    sampai = false;
                }
            }
        }
    }
}