using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenController : MonoBehaviour
{
    public GameObject uiButtons;
    public MouseInput mouseInputReference;
    // Start is called before the first frame update
    void Start()
    {
        uiButtons.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveButtons()
    {
        uiButtons.SetActive(true);
    }
    public void DeactiveButtons()
    {
        uiButtons.SetActive(false);
    }
    public void setMouseInputStateMoveToken()
    {
        mouseInputReference.setMouseState(MouseInput.MouseState.moveMVP);
        mouseInputReference.setSelectedToken(gameObject);
        DeactiveButtons();
    }
}
