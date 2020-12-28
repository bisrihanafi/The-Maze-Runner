
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
namespace Hanafi
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        #endregion

        #region Public Fields
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        [SerializeField]
        private GameObject menuRule;
        #endregion

        #region Private Fields
        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 3;
        bool isConnecting;
        #endregion

        #region MonoBehaviour CallBacks
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        private void Start()
        { 
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        #endregion

        #region MonoBehaviourPunCallbacks Callbacks


        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            // #Critical: Yang pertama kami coba lakukan adalah bergabung dengan ruang potensial yang ada. Jika ada, bagus, lain, kita akan dipanggil kembali dengan OnJoinRandomFailed ()
            if (isConnecting)
            {
                JoinKeRoom();
                isConnecting = false;
            }
        }

        void JoinKeRoom() {
            Hashtable prop=null;
            if (PlayerPrefs.GetInt("TypeLabirin") == 0)
            {
                prop = new Hashtable() 
                { 
                    { "tm", 0}, 
                    { "nc", PlayerPrefs.GetInt("NPCOnMap") } 
                };
            }
            else if (PlayerPrefs.GetInt("TypeLabirin") == 1)
            {
                prop = new Hashtable() 
                { 
                    { "tm", PlayerPrefs.GetInt("TypeLabirinDiff") + 1 },
                    { "nc", PlayerPrefs.GetInt("NPCOnMap") } 
                };
            }
            // #Critical: Yang pertama kami coba lakukan adalah bergabung dengan ruang potensial yang ada. Jika ada, bagus, lain, kita akan dipanggil kembali dengan OnJoinRandomFailed ()
            PhotonNetwork.JoinRandomRoom(prop, maxPlayersPerRoom);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayersPerRoom;
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "tm","nc" };
            if (PlayerPrefs.GetInt("TypeLabirin") == 0)
            {
                roomOptions.CustomRoomProperties = new Hashtable() 
                { 
                    { "tm", 0}, 
                    { "nc", PlayerPrefs.GetInt("NPCOnMap") } 
                };
            }
            else if (PlayerPrefs.GetInt("TypeLabirin") == 1)
            {
                roomOptions.CustomRoomProperties = new Hashtable() 
                { 
                    { "tm", PlayerPrefs.GetInt("TypeLabirinDiff")+1 },
                    {"nc", PlayerPrefs.GetInt("NPCOnMap") } 
                };
            }
            // #Critical: kami gagal bergabung dengan ruang acak, mungkin tidak ada atau semuanya penuh. Jangan khawatir, kami membuat ruangan baru.
            PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            // #Critical: Kami hanya memuat jika kami adalah pemain pertama, jika tidak, kami mengandalkan `PhotonNetwork.AutomaticallySyncScene` untuk menyinkronkan adegan instance kami.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                if (PlayerPrefs.GetInt("TypeLabirin") == 1)
                    PhotonNetwork.LoadLevel("Arena Dinamis");
                else 
                    PhotonNetwork.LoadLevel("Arena Statis");
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            if (PlayerPrefs.GetString("PlayerName").Trim().Equals(""))
            {
                menuRule.GetComponent<MainMenu>().Pengaturan();
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                progressLabel.SetActive(true);
                controlPanel.SetActive(false);
                Debug.Log("PUN Basics Tutorial/Launcher: Run connect method");
                // kami memeriksa apakah kami terhubung atau tidak, kami bergabung jika kami terhubung, jika tidak kami memulai koneksi ke server.
                if (PhotonNetwork.IsConnected)
                {
                    Debug.Log("PUN Basics Tutorial/Launcher: Photon ready was connected");
                    JoinKeRoom();
                }
                else
                {
                    Debug.Log("PUN Basics Tutorial/Launcher: PhotonNetwork.IsConnected is not connected");
                    // #Critical, pertama-tama kita harus terhubung ke Photon Online Server.
                    PhotonNetwork.ConnectUsingSettings();
                    PhotonNetwork.GameVersion = gameVersion;
                }
            }
        }
        #endregion
    }
}
