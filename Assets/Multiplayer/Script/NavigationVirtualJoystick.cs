using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NavigationVirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImg;
    private Image joy;
    public Vector3 InputDirection { set; get; }
    // Start is called before the first frame update
    void Start()
    {
        bgImg = GetComponent<Image>();
        joy = transform.GetChild(0).GetComponent<Image>();
        InputDirection = Vector3.zero;
    }

    // Update is called once per frame
    
    private void Awake()
    {
        this.transform.SetParent(GameObject.Find("Control Character Panel").GetComponent<Transform>(), false);
    }
    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
            float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;
            InputDirection = new Vector3(x, 0, y);
            InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;
            joy.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (bgImg.rectTransform.sizeDelta.x / 3), InputDirection.z * (bgImg.rectTransform.sizeDelta.y / 3));
        }
        //Debug.Log("OnDrag");
    }
    public virtual void OnPointerUp(PointerEventData ped)
    {
        InputDirection = Vector3.zero;
        joy.rectTransform.anchoredPosition = Vector3.zero;

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
