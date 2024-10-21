using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonR : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform tr;

    private void Awake() {
        tr = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tr.localScale = new Vector3(1.25f, 1.25f, 1.25f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tr.localScale = new Vector3(1f, 1f, 1f);
    }
}
