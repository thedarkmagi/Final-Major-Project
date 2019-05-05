using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleOnOff : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerExitHandler
{
    public GameObject label;
    public bool deactivated;

    // timer values
    private bool timerActive;
    private float maxTimerValue;
    private float currentTimerValue;

    // Start is called before the first frame update
    void Start()
    {
        maxTimerValue = 1;
        deactivated = false;
        timerActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerActive)
        {
            currentTimerValue -= Time.deltaTime;
            if(currentTimerValue<0)
            {
                hideLabel();
                timerActive = false;
            }
        }
    }

    public void hideLabel()
    {
        if(label.activeSelf)
        {
            label.SetActive(false);
            deactivated = true;
            tag = "PinOnly";
        }
    }
    public void rehideLabel()
    {
        if (label.activeSelf)
        {
            label.SetActive(false);
        }
    }
    public void showLabel()
    {
        if (!label.activeSelf)
        {
            label.SetActive(true);
            
        }
        tag = "Pin";
        deactivated = false;
        timerActive = false;
    }

    void showLabelTemp()
    {
        label.SetActive(true);
    }
    public void showLabelOnTimer()
    {
        showLabelTemp();
        timerActive=true;
        currentTimerValue = maxTimerValue;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //do your stuff when highlighted
        showLabel();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //do your stuff when highlighted
        if (deactivated)
        {
            rehideLabel();
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        //do your stuff when selected
        deactivated = false;
    }
}
