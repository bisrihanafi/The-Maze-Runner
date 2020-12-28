using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;


namespace Hanafi
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        
        #region Private Fields


        [SerializeField]
        private float directionDampTime = 0.25f;
        
        private NavigationVirtualJoystick joy;
        private Animator animator;
        // Use this for initialization

        #endregion

        #region MonoBehaviour Callbacks


        private void Awake()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }
            joy = GameObject.FindGameObjectWithTag("NavigationVirtualJoystick").GetComponent<NavigationVirtualJoystick>();

        }
        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }
            if (!animator)
            {
                return;
            }
            //float h = Input.GetAxis("Horizontal");
            float v = joy.InputDirection.z;

            float h = joy.InputDirection.x;
            Debug.Log(h+"----------------"+v);
            if (v < 0)
            {
                //v = 0;
                animator.SetBool("BackSteep", true);
            }else animator.SetBool("BackSteep", false);
            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }


        #endregion
        
    }
}