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
        print("we hovering");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //do your stuff when highlighted
        deleteShadow.sprite = closedIcon;
        print("we Leaving the hovering");
    }
    public void OnSelect(BaseEventData eventData)
    {
        //do your stuff when selected
        deleteShadow.sprite = closedIcon;
        print("we selecting :3");
    }
}
