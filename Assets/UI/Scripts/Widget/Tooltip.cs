using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    public GameObject overlay;
    public TextMeshProUGUI label;

    [Header("Value")]
    public string tooltip;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        GameObject obj = eventData.pointerEnter;
        overlay.SetActive(true);
        label.text = tooltip;
        RectTransform transform = overlay.GetComponent<RectTransform>();
        Vector2 size = transform.sizeDelta;
        size.x = label.preferredWidth + 20;
        transform.sizeDelta = size;
        overlay.transform.position = obj.transform.position + new Vector3(0, 10, 0);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        overlay.SetActive(false);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
