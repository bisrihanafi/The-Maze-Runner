using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Hanafi
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Field
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        #endregion

        #region Private Field
        GameObject player;
        int respownPoint;
        bool mulai;
        #endregion
        #region Photon Callbacks
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                //LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                //LoadArena();
            }
        }
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>

        //Metode override dari MonoBehaviourPunCallbacks 
        //meload scane 0 ketika meninggalkan room
        public override void OnLeftRoom(){
            SceneManager.LoadScene(0);
        }
        #endregion
        #region Public Methods
        //Metode yang dapat dipanggil secara bebas
        public void LeaveRoom(){
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region Private Methods
        /*
        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Arena");
        }
        */
        void Start()
        {
            if (PhotonNetwork.IsMasterClient) {Debug.Log("Master");}
            Debug.Log("Registered Time Delta Start " + PhotonNetwork.ServerTimestamp);
            mulai = true;
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // kita ada di kamar. menelurkan karakter untuk pemain lokal. itu akan disinkronkan dengan menggunakan PhotonNetwork.Instantiate
                    respownPoint = PhotonNetwork.CurrentRoom.PlayerCount;
                    player =PhotonNetwork.Instantiate(
                        this.playerPrefab.name, 
                        GameObject.Find("Respown"+ respownPoint).GetComponent<Transform>().position, 
                        Quaternion.identity, 
                        0
                        );
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }
        private void FixedUpdate()
        {
            if (GameObject.Find("Environment").GetComponent<GamePlay>().mulaiPlay) {
                TeleportPlayer();
            }

        }
        public void TeleportPlayer() {
            if (mulai)
            {
                string tempatPlayer = (PlayerPrefs.GetInt("TypeLabirin") == 1) ? PlayerPrefs.GetInt("TypeLabirinDiff") + 1 + "" : "";
                player.transform.position = GameObject.Find("RespownPlayer" + tempatPlayer +""+ respownPoint).GetComponent<Transform>().position;
                mulai = false;
            }
        }
        #endregion
    }
}