using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteIconShadowController : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerExitHandler
{

    public Sprite openIcon, closedIcon;
    public Image deleteShadow;

    void Start()
    {
        //deleteShadow = GetComponentInChildren<Image>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //do your stuff when highlighted
        deleteShadow.sprite = openIcon;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //do your stuff when highlighted
        deleteShadow.sprite = closedIcon;
    }
    public void OnSelect(BaseEventData eventData)
    {
        //do your stuff when selected
        deleteShadow.sprite = closedIcon;
    }
}
