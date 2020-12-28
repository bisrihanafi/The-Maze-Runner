using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace Hanafi
{
    public class MenuScript : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject MenuPanel;
        //[SerializeField]
        //private GameObject MasterPanel;
        [SerializeField]
        private GameObject labirinGenerate;
        // Start is called before the first frame update
        void Start()
        {
            
            MenuPanel.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
            {
                if (PlayerPrefs.GetInt("TypeLabirin") == 1)
                {
                    //MasterPanel.SetActive(true);
                    int difficult = PlayerPrefs.GetInt("TypeLabirinDiff");
                    if (difficult == 0)
                    {
                        labirinGenerate.GetComponent<MazeGenerator>().Simpel();
                    }
                    else if (difficult == 1)
                    {
                        labirinGenerate.GetComponent<MazeGenerator>().Sedang();
                    }
                    else if (difficult == 2)
                    {
                        labirinGenerate.GetComponent<MazeGenerator>().Kompleks();
                    }
                }
            }
            //else MasterPanel.SetActive(false);
            
        }

        // Update is called once per frame
        
        public void OpenMenuPanel()
        {
            MenuPanel.SetActive(true);
        }
        public void CloseMenuPanel()
        {
            MenuPanel.SetActive(false);
        }
        public void CloseMasterPanel()
        {
            //MasterPanel.SetActive(false);
        }
    }
}