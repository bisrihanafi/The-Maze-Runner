using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AngleVirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField]
    private float speed;
    ///private Image joy;
    public Vector3 InputDirection { set; get; }
    // Start is called before the first frame update

    private void Awake()
    {
        this.transform.SetParent(GameObject.Find("Control Character Panel").GetComponent<Transform>(), false);
    }
    void Start()
    {
        //bgImg = GetComponent<Image>();
        //joy = transform.GetChild(0).GetComponent<Image>();
        InputDirection = Vector3.zero;
    }

    // Update is called once per frame
    
    public virtual void OnDrag(PointerEventData ped)
    {
        InputDirection = ped.delta*Time.deltaTime*speed; 
    }
    public virtual void OnPointerUp(PointerEventData ped)
    {
        InputDirection = Vector3.zero;
        //joy.rectTransform.anchoredPosition = Vector3.zero;

    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);

    }
}
/**
 * for code on the controller add
 *  public VirtualJoystick moveJoystick;
 * 
 * add after navigation script
 * if (moveJoystick.InputDirection != Vector3.zero){
 * 		dir = moveJoystick.InputDirection;
 * }
 * */
