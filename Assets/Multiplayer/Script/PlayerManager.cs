using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

using Pathfinding;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace Hanafi
{

    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Private Fields
        private Vector3 moveDirection = Vector3.zero;
        CharacterController characterController;
        [SerializeField]
        private GameObject joy;
        [SerializeField]
        private GameObject angle;
        [SerializeField]
        private GameObject actionButton1;
        [SerializeField]
        private GameObject hit;
        private NavigationVirtualJoystick joy_v;
        private AngleVirtualJoystick angle_v;
        private TapButton actionButton1_v;
        private Animator animator;
        private float directionDampTime = 0.25f;
        private uint count_delay;
        private float total_delay;
        private Text status_syns;
        private string string_avg;
        private bool hit_v;
        bool recoverHP,sudahSave;
        GamePlay gamePlay;
        GameObject _uiGo, _uiAng, _uiJoy, _uiAcB, _uiGoOther;
        private Text status_ping;
        #endregion

        #region Public Fields
        [Tooltip("The current Health of our player")]
        public float Health = 1f;
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject playerHp;
        [SerializeField]
        public GameObject OtherHp;
        public float speed = 6.0f;
        public float gravity = 20.0f;
        float timeLeft = 10.0f;
        private int debugMode=0;
        int ping=0;
        LogSaverAndSender logSave;

        //UI untuk menampilkan Game Over
        GameObject hpCritical;
        GameObject panelWinner;
        GameObject death;
        Text winnerCountDown, deathCountDown;

        //bagian ini untuk menampilkan jarak Player dengan NPC secara radius
        Text remNPC1, remNPC2, remNPC3;
        GameObject[] NPC;
        #endregion

        #region MonoBehaviour CallBacks
        void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            logSave = GameObject.Find("Log Saver").GetComponent<LogSaverAndSender>();
            sudahSave = false;
            debugMode = PlayerPrefs.GetInt("DebugerMode");
            recoverHP = true;
            hit_v = false;
            total_delay = 0;
            count_delay = 0;
            CameraForwad _cameraWork = this.gameObject.GetComponent<CameraForwad>();
            characterController = GetComponent<CharacterController>();
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerName");
            if (photonView.IsMine && actionButton1 != null && joy != null && angle != null 
                && playerHp != null && _cameraWork != null) {
                //membuat HP bar curret Player
                _uiGo = Instantiate(playerHp);
                //membuat controller
                _uiAng = Instantiate(angle);
                _uiJoy = Instantiate(joy);
                _uiAcB = Instantiate(actionButton1);
                _cameraWork.OnStartFollowing();
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                joy_v = _uiJoy.GetComponent<NavigationVirtualJoystick>();
                angle_v = _uiAng.GetComponent<AngleVirtualJoystick>();
                actionButton1_v = _uiAcB.GetComponent<TapButton>();
            }
            else if (OtherHp != null)
            {
                //membuat tampilan HP bar di Player lain
                _uiGoOther = Instantiate(OtherHp);
                _uiGoOther.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
            if (photonView.IsMine) {
                gamePlay = GameObject.Find("Environment").GetComponent<GamePlay>();
                hpCritical = GameObject.Find("BloodDmg");
                death = GameObject.Find("Game Over");
                panelWinner = GameObject.Find("WinnerPanel");
                winnerCountDown = GameObject.Find("WinnerCountDown").GetComponent<Text>();
                deathCountDown = GameObject.Find("Button Return to Menu").GetComponent<Text>();
                hpCritical.SetActive(false);
                death.SetActive(false);
                panelWinner.SetActive(false);

                //bagian radar NPC
                remNPC1 = GameObject.Find("RemNPC1").GetComponent<Text>();
                remNPC2 = GameObject.Find("RemNPC2").GetComponent<Text>();
                remNPC3 = GameObject.Find("RemNPC3").GetComponent<Text>();
            }
            status_ping = GameObject.Find("Ping").GetComponent<Text>();
            status_syns = GameObject.Find("Time Latency" + photonView.Owner.ActorNumber).GetComponent<Text>();
            string_avg = "0"; 
        }
        void Update()
        {
            if (photonView.IsMine)
            {
                if (GameObject.FindGameObjectsWithTag("Winner").Length != 0 && !gameObject.tag.Trim().Equals("Winner"))
                {
                    MeGameOver();
                    return;
                }
                if (gameObject.tag.Trim().Equals("Winner"))
                {
                    SimpanData();
                    timeLeft -= Time.deltaTime;
                    winnerCountDown.text = Mathf.Round(timeLeft) + "";
                    panelWinner.SetActive(true);
                    if (timeLeft < 0)
                    {
                        PhotonNetwork.LeaveRoom();
                    }
                    return;
                }
                if (gameObject.tag.Trim().Equals("Player Death"))
                {
                    
                    timeLeft -= Time.deltaTime;
                    deathCountDown.text = "To Menu (" + Mathf.Round(timeLeft) + ")";
                    if (timeLeft < 0)
                    {
                        PhotonNetwork.LeaveRoom();
                    }
                    return;
                }
            }
            ProcessInputs();
            hit.SetActive(hit_v);
        }
        void SimpanData() {
            if (!sudahSave)
            {
                logSave.SimpanData();
                sudahSave = true;
            }
        }
        private void FixedUpdate()
        {
            ping = PhotonNetwork.GetPing();
            if (photonView.IsMine)
            {
                if (ping > 300) status_ping.color = Color.red;
                else if (ping > 100) status_ping.color = Color.yellow;
                else if (ping > 50) status_ping.color = Color.white;
                else if (ping > 0) status_ping.color = Color.green;
                status_ping.text = ping + "";
                if (debugMode == 0)
                {
                    RemeberSetUI();
                }
                if (gamePlay.mulaiPlay)
                {
                    RecoverHp();
                }
                if (Health < 0.2f && Health > 0)
                {
                    Berdarah(true);
                }
                else if (Health <= 0f)
                {
                    MeGameOver();
                }
                /*
                // bagian auto cover HP ketika HP 50% digunakan untuk membuat player tidak bisam mati
                if (debugMode == 1 && Health<0.5) {
                    Health = 1;
                }
                */
            }
        }

        //Penerima demage
        void OnTriggerEnter(Collider other){
            if (other.tag.Trim().Equals("Finish")) {
                //script untuk finish
                gameObject.tag = "Winner";
                speed = 0;
            }
            if (!photonView.IsMine){return;}
            else if (photonView.IsMine && other.name.Trim().Equals("Hit")){
                if (gameObject.tag.Equals("Player")){
                    Health -= 0.2f * Time.deltaTime;
                }   
            } 
        }
        void OnTriggerStay(Collider other){
            if (!photonView.IsMine){return;}
            else if (photonView.IsMine &&  other.tag.Trim().Equals("Robot")){
                if (gameObject.tag.Equals("Player")){
                    Health -= 0.1f * Time.deltaTime;
                }
            } 
        }
        #endregion

        #region Custom
        void ProcessInputs()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) return;
            if (!animator)return;

            float v = joy_v.InputDirection.z;
            float h = joy_v.InputDirection.x;
            float r = angle_v.InputDirection.x;
            
            if (characterController.isGrounded)
            {
                
                moveDirection = new Vector3(h, 0.0f, v);
                moveDirection *= speed;
                moveDirection.y -= gravity * Time.deltaTime;
                characterController.Move(transform.TransformDirection(moveDirection * Time.deltaTime*speed));
                characterController.transform.Rotate(0,r* speed*2f, 0);
                if (v < 0)
                {
                    animator.SetBool("BackSteep", true);
                }
                else animator.SetBool("BackSteep", false);
                animator.SetFloat("Speed", h * h + v * v);
                animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
                hit_v = actionButton1_v.InputAction;
            }
        }
        void RemeberSetUI() {
            NPC = GameObject.FindGameObjectsWithTag("Robot");
            if (NPC.Length == 1)
            { // jika NPC hanya satu
                remNPC1.text = Math.Round(Jarak(NPC[0].GetComponent<Transform>(), gameObject.transform))+" : Robot1";
            }
            else if (NPC.Length == 2)
            { // jika NPC berjumlah 2
                remNPC1.text = Math.Round(Jarak(NPC[0].GetComponent<Transform>(), gameObject.transform)) + " : Robot1";
                remNPC2.text = Math.Round(Jarak(NPC[1].GetComponent<Transform>(), gameObject.transform)) + " : Robot2";
            }
            else if (NPC.Length == 3)
            { // Jika NPC berjumlah 3
                remNPC1.text = Math.Round(Jarak(NPC[0].GetComponent<Transform>(), gameObject.transform))+ " : Robot1";
                remNPC2.text = Math.Round(Jarak(NPC[1].GetComponent<Transform>(), gameObject.transform)) + " : Robot2";
                remNPC3.text = Math.Round(Jarak(NPC[2].GetComponent<Transform>(), gameObject.transform))+ " : Robot3";
            }
        }
        double Jarak(Transform position1, Transform position2)
        {
            double a1 = position1.position.x;
            double b1 = position1.position.z;
            double a2 = position2.position.x;
            double b2 = position2.position.z;
            double tmp = b1 - b2;
            double dist = (Math.Sin(a1 * Mathf.Deg2Rad) * Math.Sin(a2 * Mathf.Deg2Rad)) + (Math.Cos(a1 * Mathf.Deg2Rad) * Math.Cos(a2 * Mathf.Deg2Rad) * Math.Cos(tmp * Mathf.Deg2Rad));
            dist = Math.Acos(dist);
            dist = dist * Mathf.Rad2Deg;
            return (dist);
        }
        public void RecoverHp()
        {
            if (recoverHP)
            {
                this.Health = 1.0f;
                recoverHP = false;
            }
        }
        void Berdarah(bool i)
        {
            if (photonView.IsMine)
            {
                hpCritical.SetActive(i);
            }
        }

        void Mati(bool i)
        {
            if (photonView.IsMine)
            {
                death.SetActive(i);
            }
        }
        void MeGameOver() {
            SimpanData();
            speed = 0f;
            Berdarah(false);
            Mati(true);
            if (!gameObject.tag.Equals("Player Death"))
            {
                _uiAng.SetActive(false);
                _uiJoy.SetActive(false);
                _uiAcB.SetActive(false);
                gameObject.tag = "Player Death";
            }
        }
        /*
        void CalledOnLevelWasLoaded() {
            GameObject _uiGo = Instantiate(this.PlayerUiHpPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            GameObject _uiJoy = Instantiate(this.joy);
            //_uiJoy.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            GameObject _uiAng = Instantiate(this.angle);
            //_uiAng.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

        }
        */
        #endregion
        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
            if (stream.IsWriting){
                // We own this player: send the others our data
                //mengirim data attack
                stream.SendNext(hit_v);
                //mengirim data HP
                stream.SendNext(Health);
                //mengirim data tag
                stream.SendNext(gameObject.tag);
            }else{
                //menerima data attack
                hit_v = (bool)stream.ReceiveNext();
                //menrima data HP
                this.Health = (float)stream.ReceiveNext();
                //menghitung waktu yang dibutuhkan untuk mengirim data
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
                count_delay++;
                string fill_log = "Registered Name " 
                    + this.photonView.Owner.NickName + " "
                    + count_delay + " " + ((int)(lag * 1000f)) + " " 
                    + ping;
                Debug.Log(fill_log);
                status_syns.text = fill_log;
                //menerima data tag
                this.gameObject.tag = (string) stream.ReceiveNext();
            }   
        }
        
        #endregion
    }
}