using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Hanafi
{
    public class GamePlay : MonoBehaviour
    {

        #region Field
        GameObject countDown,readykuy;
        float timeLeft = 100.0f;
        float timeLeftReady = 5.0f;
        public bool mulaiPlay;
        [SerializeField] GameObject aStart;
        [SerializeField] GameObject npc1, npc2, npc3;

        //Exekutor bool
        bool eksekusiSudah;
        #endregion
        private void Start()
        {
            countDown = GameObject.Find("CoolDown");
            readykuy= GameObject.Find("KeteranganUI");
            //demageEffect = GameObject.Find("BloodDmg");
            mulaiPlay = false;
            eksekusiSudah = false;
        }
        private void Update()
        {
            if (!eksekusiSudah)
            {
                if (PlayerPrefs.GetInt("DebugerMode") == 1)
                {
                    timeLeftReady -= Time.deltaTime;
                    countDown.GetComponent<Text>().text = "GO!!!";
                    if (timeLeftReady < 0)
                    {
                        GoPlay();
                    }
                }
                else if (PlayerPrefs.GetInt("DebugerMode") == 0)
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                    {
                        countDown.GetComponent<Text>().text = "....";
                        timeLeft = 100.0f;
                    }
                    else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                    {
                        timeLeft -= Time.deltaTime;
                        countDown.GetComponent<Text>().text = "" + Mathf.Round(timeLeft);
                        if (timeLeft < 0)
                        {
                            GoPlay();
                        }
                    }
                    else if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
                    {
                        timeLeftReady -= Time.deltaTime;
                        countDown.GetComponent<Text>().text = "GO!!!";
                        if (timeLeftReady < 0)
                        {
                            GoPlay();
                        }
                    }
                }
            }
        }
        void GoPlay() {
            countDown.SetActive(false);
            readykuy.SetActive(false);
            //Menutup koneksi player baru
            PhotonNetwork.CurrentRoom.IsOpen = false;
            //membuat room menjadi tidak terlihat oleh player baru
            PhotonNetwork.CurrentRoom.IsVisible = false;
            mulaiPlay = true;
            CreateNPC(PlayerPrefs.GetInt("NPCOnMap"));
            eksekusiSudah = true;     
        }
        public void CreateNPC(int i){ 
            string tempatNPC = 
                (PlayerPrefs.GetInt("TypeLabirin") == 1) ? 
                PlayerPrefs.GetInt("TypeLabirinDiff") + 1 + "" : "";
            PhotonNetwork.InstantiateSceneObject(aStart.name, Vector3.zero, Quaternion.identity);
            if (i == 1){
                PhotonNetwork.InstantiateSceneObject(
                    npc1.name, 
                    GameObject.Find("RespownNPC"+ tempatNPC+"1").GetComponent<Transform>().position, 
                    Quaternion.identity);
            }else if (i == 2){
                PhotonNetwork.InstantiateSceneObject(
                    npc1.name, 
                    GameObject.Find("RespownNPC" + tempatNPC + "1").GetComponent<Transform>().position, 
                    Quaternion.identity);
                PhotonNetwork.InstantiateSceneObject(
                    npc2.name, 
                    GameObject.Find("RespownNPC" + tempatNPC + "2").GetComponent<Transform>().position, 
                    Quaternion.identity);
            }else if (i == 3){
                PhotonNetwork.InstantiateSceneObject(
                    npc1.name, 
                    GameObject.Find("RespownNPC" + tempatNPC + "1").GetComponent<Transform>().position, 
                    Quaternion.identity);
                PhotonNetwork.InstantiateSceneObject(
                    npc2.name, 
                    GameObject.Find("RespownNPC" + tempatNPC + "2").GetComponent<Transform>().position, 
                    Quaternion.identity);
                PhotonNetwork.InstantiateSceneObject(
                    npc3.name, 
                    GameObject.Find("RespownNPC" + tempatNPC + "3").GetComponent<Transform>().position, 
                    Quaternion.identity);
            }
        }
  

    }
}