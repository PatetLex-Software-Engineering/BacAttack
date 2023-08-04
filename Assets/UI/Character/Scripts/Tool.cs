using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class Tool : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler
{

    public static Tool current;

    [Header("Components")]
    public UnityEvent use;

    [Header("Values")]
    public float atpCost;
 
    private Vector3 origin;

    private bool locked;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        if (current == null && !locked) {
            current = this;
        }
    }

    void IPointerMoveHandler.OnPointerMove(PointerEventData eventData) {
        if (current != null) {
            current.gameObject.GetComponent<RectTransform>().position = eventData.position + new Vector2(50, 50);
        }
    }

    void Start()
    {
        this.origin = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
    }

    void Update()
    {
        if (current != null) {
            if (!Input.GetMouseButton(0)) {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 20)) {
                    use.Invoke();
                }

                current.gameObject.GetComponent<RectTransform>().anchoredPosition = this.origin;
                current = null;
            }
        }
    }

    public void SetLocked(bool locked) {
        this.locked = locked;
    }
}
