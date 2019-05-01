using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InputFieldController : MonoBehaviour
{
    private TMP_InputField inputField;
    private Text buttonText;
    public GameObject deleteButton;
    private void Start()
    {
        deleteButton.SetActive(false);
        buttonText = GetComponentInChildren<Text>();
        inputField = GetComponentInChildren<TMP_InputField>();
    }
    public void setReadOnly()
    {
        if (inputField.interactable)
        {
            inputField.interactable = false;
            buttonText.text = "Edit";
            deleteButton.SetActive(false);
        }
        else
        {
            inputField.interactable = true;
            buttonText.text = "Done";
            deleteButton.SetActive(true);

        }
    }
}
