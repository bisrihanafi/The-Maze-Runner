using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    
    public bool InputAction { set; get; }

    private void Awake()
    {
        this.transform.SetParent(GameObject.Find("Control Character Panel").GetComponent<Transform>(), false);

    }
    // Start is called before the first frame update
    

    // Update is called once per frame
    
    public void OnPointerDown(PointerEventData eventData)
    {
        InputAction = true;
        Debug.Log("Input Action true");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputAction = false;
        Debug.Log("Input Action false");
    }
}
