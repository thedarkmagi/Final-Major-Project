using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenController : MonoBehaviour
{
    public List<GameObject> uiButtons;
    public MouseInput mouseInputReference;
    // Start is called before the first frame update
    void Start()
    {
        setButtionsState(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveButtons()
    {
        setButtionsState(true);
    }
    public void DeactiveButtons()
    {
        setButtionsState(false);
    }
    private void setButtionsState(bool state)
    {
        for (int i = 0; i < uiButtons.Count; i++)
        {
            uiButtons[i].SetActive(state);
        }
    }
    public void setMouseInputStateMoveToken()
    {
        mouseInputReference.setMouseState(MouseInput.MouseState.moveMVP);
        mouseInputReference.setSelectedToken(gameObject);
        DeactiveButtons();
    }
    public void killToken()
    {
        Destroy(gameObject);
    }
}
