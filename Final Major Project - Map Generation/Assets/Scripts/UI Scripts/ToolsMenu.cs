using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToolsMenu : MonoBehaviour
{
    public GameObject panelReference;
    private Text buttonText;
    // Start is called before the first frame update
    void Start()
    {
        buttonText = GameObject.Find("ToolText").GetComponent<Text>();
        panelReference.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void enableTools()
    {
        panelReference.SetActive(!panelReference.activeSelf);
        if(!panelReference.activeSelf)
        {
            buttonText.text = "Tools";
        }
        else
        {
            buttonText.text = "Close Tools";
        }
    }
}
