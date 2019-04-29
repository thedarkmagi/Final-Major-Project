using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InputFieldController : MonoBehaviour
{
    private TMP_InputField inputField;
    private Text buttonText;
    private void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        inputField = GetComponentInChildren<TMP_InputField>();
    }
    public void setReadOnly()
    {
        if (inputField.interactable)
        {
            inputField.interactable = false;
            buttonText.text = "Edit";
        }
        else
        {
            inputField.interactable = true;
            buttonText.text = "Done";
        }
    }
}
