using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

using System.Collections;


namespace Hanafi
{
    public class PlayerUIHP : MonoBehaviour
    {
        #region Private Fields


        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        private Text playerNameText;


        [Tooltip("UI Slider to display Player's Health")]
        [SerializeField]
        private Slider playerHealthSlider;
        private PlayerManager target;
        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;
        private GameObject HPOther1,HPOther2;
        #endregion

        #region Public Field
        [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
        #endregion

        #region MonoBehaviour Callbacks

        void Update()
        {
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }
            // Reflect the Player Health
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = target.Health;
            }
           
            //Debug.Log("-----------------> " + HPOther1.transform.childCount);
        }

        void Awake()
        {
            
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            HPOther1 = GameObject.Find("HP Other Player 1");
            HPOther2 = GameObject.Find("HP Other Player 2");
            if (target.photonView.IsMine)
            {
                this.transform.SetParent(GameObject.Find("UI").GetComponent<Transform>(), false);
            }
            else
            {
                if (HPOther1.transform.childCount == 0)
                {
                    this.transform.SetParent(GameObject.Find("HP Other Player 1").GetComponent<Transform>(), false);
                }else if (HPOther1.transform.childCount == 1)
                {
                    this.transform.SetParent(GameObject.Find("HP Other Player 2").GetComponent<Transform>(), false);
                }
                
            }            
        }
        void LateUpdate()
        {
            // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
            if (targetRenderer != null)
            {
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            // #Critical
            // Follow the Target GameObject on screen.
            /*
            if (targetTransform != null && !target.photonView.IsMine)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
                
            }
            */
            
        }
        #endregion


        #region Public Methods

        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            // Cache references for efficiency
            target = _target;
            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }
            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();
            // Get data from the Player that won't change during the lifetime of this Component
            if (characterController != null)
            {
                characterControllerHeight = characterController.height;
            }
        }

        #endregion


    }
}